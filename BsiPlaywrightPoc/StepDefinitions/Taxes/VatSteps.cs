using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.Enums;
using BsiPlaywrightPoc.Model.RequestObject;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Taxes
{
    [Binding]
    public sealed class VatSteps(
        ScenarioContext scenarioContext,
        LoginPage loginPage,
        HomePage homePage,
        SearchPage searchPage,
        ProductPage productPage,
        TaxPage taxPage,
        BasketPage basketPage,
        PaymentInformationPage paymentInformationPage,
        PaymentPage paymentPage,
        MiddlewareHelper middlewareHelper,
        CustomerProfileHelper customerProfileHelper,
        ILogger logger)
    {
        private UserCredentials? _randomUser;
        private UserAddressDetails? _addressDetails;
        private decimal _totalCostBeforeVat;
        private decimal _totalCostAfterVat;
        private decimal _subTotalPrice;
        private decimal _estimatedTaxes;
        private bool _vatWasDefinedAtCheckout;
        private bool _isCodiceFiscalePath;
        private AppSettings? _appSettings;

        [Given(@"I am on checkout page as an '([^']*)' user with vat '([^']*)' on my account profile")]
        public async Task GivenIAmOnCheckoutPageAsAnUserWithOnMyAccountProfile(string persona, string vatDefinition)
        {
            await SetUpUserAndNavigateToCheckout(persona, vatDefinition);
        }

        [Given(@"I am on checkout page as an Italian user with codice fiscale '([^']*)' on my account profile")]
        public async Task GivenIAmOnCheckoutPageAsAnUserWithCodiceFiscaleOnMyAccountProfile(string codiceFiscaleDefined)
        {
            _isCodiceFiscalePath = true;
            await SetUpUserAndNavigateToCheckout("italian", null, codiceFiscaleDefined);
        }

        [When(@"I '([^']*)' the vat number at checkout")]
        public async Task WhenITheVatNumberAtCheckout(string shouldDefinedVatAtCheckout)
        {
            await paymentInformationPage.ClickContinueToPayment();

            if (shouldDefinedVatAtCheckout.ToLower() == "defined")
            {
                _subTotalPrice = decimal.Parse(await paymentPage.GetSubTotalPrice());
                _estimatedTaxes = decimal.Parse(await paymentPage.GetEstimatedTaxes());
                _totalCostAfterVat = decimal.Parse(await paymentPage.GetTotalPrice());

                await paymentPage.ClickChangeVatNumber();
                if (_isCodiceFiscalePath)
                {
                    await taxPage.EditCodiceFiscale(_addressDetails?.CodiceFiscale!);
                }
                else
                {
                    await taxPage.EditVatNumber(_addressDetails?.Vat!, _addressDetails?.VatType!);
                }

                await basketPage.ClickBasketIcon();
                await basketPage.ProceedToCheckout();
                _vatWasDefinedAtCheckout = true;
            }
        }

        [Then(@"the VAT should be applied correctly as expected based on vat was '([^']*)' for '([^']*)'")]
        public async Task ThenTheVATShouldBeAppliedCorrectlyAsExpectedBasedOnOnMyProfile(string wasVatDefined, string persona)
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            if (wasVatDefined.ToLower() == "defined")
            {
                await ValidateVatForDefinedUser();
            }
            else
            {
                await ValidateVatForNotDefinedUser(persona);
            }

            var vatNumberFriendlyMessage = await paymentPage.GetVatFriendlyMessage();
            vatNumberFriendlyMessage.Should().Be("The VAT number is used to identify your tax status, helps to identify the place of taxation and is included on your invoices. If you wish to edit or remove the VAT number for any reason, please do so on your profile page.");
        }

        [Then(@"the purchase should be '([^']*)'")]
        public async Task ThenThePurchaseShouldBe(string ableToCarryOutPurchase)
        {
            var isPurchaseAllowed = ableToCarryOutPurchase.ToLower() == "disallowed";
            var isPayNowButtonVisible = await paymentPage.IsPayNowButtonDisabled();
            isPayNowButtonVisible.Should().Be(isPurchaseAllowed, $"Pay Now Button visibility wasn't as expected.");
        }

        // Helper method to set up a user and navigate to checkout
        private async Task SetUpUserAndNavigateToCheckout(string persona, string? vatDefinition = null, string? codiceFiscaleDefined = null)
        {
            _randomUser = RandomData.GenerateRandomUserCredentials();
            _addressDetails = persona.ToLower() switch
            {
                "irish" => RandomData.GetAddressDetails("Ireland"),
                "english" => RandomData.GetAddressDetails("United Kingdom"),
                "italian" => RandomData.GetAddressDetails("Italy"),
                "german" => RandomData.GetAddressDetails("Germany"),
                _ => throw new ArgumentException("Invalid user persona requested")
            };

            var payload = new UserSignUpRequestObject()
            {
                email = _randomUser.Email,
                firstName = _randomUser.Firstname,
                lastName = _randomUser.Lastname,
                password = _randomUser.Password
            };

            await middlewareHelper.SignUp(payload);
            await loginPage.Login(_randomUser.Email!, _randomUser.Password!);

            if (vatDefinition == "defined" || codiceFiscaleDefined == "defined")
            {
                await SetVatOrCodiceFiscale(vatDefinition!, codiceFiscaleDefined!);
            }

            const PurchaseType purchaseType = PurchaseType.DigitalCopy;
            var standardToBeAddedToBasket = purchaseType.GetRandomStandardSapId();

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(standardToBeAddedToBasket);
            await productPage.AddDisplayedStandardAndProceedToBasket(purchaseType, 1);
            await basketPage.ProceedToCheckout();

            // grab the total prices before changing anything
            _totalCostBeforeVat = decimal.Parse(await paymentPage.GetTotalPrice());
            await paymentInformationPage.FillOutPaymentInformationForm(_addressDetails!, _randomUser?.Firstname!, _randomUser?.Lastname!);
        }

        private async Task SetVatOrCodiceFiscale(string? vatDefinition, string? codiceFiscaleDefined)
        {
            if (vatDefinition != null)
            {
                var payload = new VatRequestObject()
                {
                    email = _randomUser!.Email,
                    vatNumber = _addressDetails!.Vat,
                    vatNumberType = _addressDetails?.VatType?.ToLower() == "eu" ? "eu" : "uk"
                };

                await Task.Delay(5000); // Wait for system sync
                await customerProfileHelper.SetUpVat(payload);
            }
            else if (codiceFiscaleDefined != null)
            {
                var payload = new CodiceFiscaleRequestObject()
                {
                    email = _randomUser!.Email,
                    codiceFiscale = _addressDetails!.CodiceFiscale
                };

                await Task.Delay(5000); // Wait for system sync
                await customerProfileHelper.SetUpCodiceFiscale(payload);
            }
        }

        private async Task ValidateVatForNotDefinedUser(string persona)
        {
            if (!_vatWasDefinedAtCheckout)
            {
                _subTotalPrice = decimal.Parse(await paymentPage.GetSubTotalPrice());
                _estimatedTaxes = decimal.Parse(await paymentPage.GetEstimatedTaxes());
                _totalCostAfterVat = decimal.Parse(await paymentPage.GetTotalPrice());
            }

            switch (persona.ToLower())
            {
                case "german":
                case "irish":
                case "italian":
                    _totalCostBeforeVat.Should().Be(_subTotalPrice, "VAT was not applied correctly");
                    _totalCostBeforeVat.Should().BeLessThan(_totalCostAfterVat, "VAT was not applied correctly");
                    _totalCostAfterVat.Should().Be(_estimatedTaxes + _subTotalPrice, "VAT was not applied correctly");
                    break;

                case "english":
                    _totalCostAfterVat = decimal.Parse(await paymentPage.GetTotalPrice());
                    _totalCostBeforeVat.Should().Be(_totalCostAfterVat, "VAT was not applied correctly");
                    break;
            }
        }

        private async Task ValidateVatForDefinedUser()
        {
            if (_isCodiceFiscalePath)
            {
                var isCodiceFiscaleNumberVisible = await paymentPage.IsVatNumberVisible(_addressDetails?.CodiceFiscale!);
                isCodiceFiscaleNumberVisible.Should().BeTrue("Codice Fiscale number was not visible");
            }
            else
            {
                var isVatNumberVisible = await paymentPage.IsVatNumberVisible(_addressDetails?.Vat!);
                isVatNumberVisible.Should().BeTrue("Vat number was not visible");
            }

            var expectedVatOrCodiceFiscale =
                _isCodiceFiscalePath ? _addressDetails?.CodiceFiscale : _addressDetails?.Vat!;

            var customerProfile = _randomUser?.Email?.QueryCustomerProfile(_appSettings!, logger);
            customerProfile?.Rows.Count.Should().Be(1);
            var vatOrCodiceFiscaleNumberInTheDb = _isCodiceFiscalePath ? customerProfile?.Rows[0]["CodiceFiscale"].ToString() : customerProfile?.Rows[0]["VatNumber"].ToString();
            expectedVatOrCodiceFiscale.Should().Be(vatOrCodiceFiscaleNumberInTheDb, $"VAT or Codice Fiscale number in database doesn't match for user: {_randomUser?.Email}");
        }
    }
}
