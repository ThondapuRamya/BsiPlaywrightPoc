using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.RequestObject;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using BsiPlaywrightPoc.Model.AppSettings;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;
using BsiPlaywrightPoc.Model.Enums;
using BsiPlaywrightPoc.TestData;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class KnowledgeUserHooks(
        ScenarioContext scenarioContext,
        MiddlewareHelper middlewareHelper,
        LoginPage loginPage,
        HomePage homePage,
        SearchPage searchPage,
        ProductPage productPage,
        BasketPage basketPage,
        PaymentInformationPage paymentInformationPage,
        PaymentPage paymentPage,
        PaymentConfirmationPage paymentConfirmationPage,
        IUnitTestRuntimeProvider unitTestRuntimeProvider)
    {
        private AppSettings? _appSettings;

        [BeforeScenario("@CreateRandomUserAndLogin", Order = HookOrdering.User)]
        public async Task SetUpAndLoginRandomUserBeforeScenario()
        {
            var payBy = "Credit Card";
            UserCredentials randomUser;
            _appSettings = scenarioContext.Get<AppSettings>();

            // Check if 'payBy' exists in the ScenarioContext
            var scenarioTitle = scenarioContext.ScenarioInfo.Title;
            switch (scenarioTitle.ToLower())
            {
                case "purchasing a standard":
                    {
                        var arguments = scenarioContext.ScenarioInfo.Arguments;
                        if (arguments != null && arguments.Contains("payBy"))
                        {
                            payBy = arguments["payBy"]?.ToString();

                            if (payBy!.Equals("invoice", StringComparison.OrdinalIgnoreCase) && _appSettings.RuntimeSettings.Environment.ToLower() != "uat")
                            {
                                unitTestRuntimeProvider.TestInconclusive(
                                    $"{scenarioTitle} Scenario for {payBy} user, is not set-up on {_appSettings.RuntimeSettings.Environment} environment, so therefor it needs to be skipped {Environment.NewLine}");
                            }
                        }
                        break;
                    }
            }

            switch (payBy.ToLower())
            {
                case "invoice":
                    randomUser = RandomData.GetInvoiceUserDetails();
                    break;
                default:
                    {
                        randomUser = RandomData.GenerateRandomUserCredentials();
                        var payload = new UserSignUpRequestObject()
                        {
                            email = randomUser.Email,
                            firstName = randomUser.Firstname,
                            lastName = randomUser.Lastname,
                            password = randomUser.Password
                        };

                        await middlewareHelper.SignUp(payload);
                        break;
                    }
            }

            await loginPage.Login(randomUser.Email!, randomUser.Password!);

            var isUserLoggedIn = await homePage.IsLoggedInAsync();
            isUserLoggedIn.Should().BeTrue();

            scenarioContext.Set(randomUser);
        }

        [BeforeScenario("@purchasedStandard", Order = HookOrdering.User)]
        public async Task PurchaseStandardBeforeScenarioStarts()
        {
            var addressDetails = RandomData.GetAddressDetails("United Kingdom");
            var cardDetails = RandomData.GenerateCreditCardDetails();
            var randomUser = RandomData.GenerateRandomUserCredentials();
            var payload = new UserSignUpRequestObject()
            {
                email = randomUser.Email,
                firstName = randomUser.Firstname,
                lastName = randomUser.Lastname,
                password = randomUser.Password
            };

            await middlewareHelper.SignUp(payload);
            await loginPage.Login(randomUser.Email!, randomUser.Password!);

            const PurchaseType purchaseType = PurchaseType.DigitalCopy;
            var standardToBeAddedToBasket = purchaseType.GetRandomStandardSapId();

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(standardToBeAddedToBasket);
            await productPage.AddDisplayedStandardAndProceedToBasket(purchaseType, 1);
            await basketPage.ProceedToCheckout();
            await paymentInformationPage.FillOutPaymentInformationForm(addressDetails, randomUser.Firstname!, randomUser.Lastname!);
            await paymentInformationPage.ClickContinueToPayment();
            await paymentPage.FillOutPaymentInformationForm(cardDetails.Number!, cardDetails.Expiry!, cardDetails.CVV!, $"{randomUser.Firstname} {randomUser.Lastname}");
            await paymentPage.ClickPayNow();

            var orderNumber = await paymentConfirmationPage.GetOrderNumber();
            orderNumber.Should().NotBeNullOrEmpty();

            // let get back to the home page
            await paymentConfirmationPage.ClickContinueShopping();

            // lets hold all we need in memory
            scenarioContext.Set(randomUser);
            scenarioContext.Set(standardToBeAddedToBasket, "PurchasedSapId");
            scenarioContext.Set(orderNumber, "OrderNumber");
        }
    }
}
