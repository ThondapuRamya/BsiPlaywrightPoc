using System.Text.Json;
using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.Enums;
using BsiPlaywrightPoc.Model.ResponseObjects.SapDwh;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BsiPlaywrightPoc.StepDefinitions.Checkout
{
    [Binding]
    public sealed class CheckoutSteps(
        ScenarioContext scenarioContext,
        HomePage homePage,
        SearchPage searchPage,
        ProductPage productPage,
        BasketPage basketPage,
        PaymentInformationPage paymentInformationPage,
        PaymentPage paymentPage,
        PaymentConfirmationPage paymentConfirmationPage,
        ShippingPage shippingPage,
        ILogger logger)
    {
        private PurchaseType _purchaseType;
        private string? _standardSapIdToBePurchased;
        private int _standardQuantityPurchased;
        private string _modeOfPayment;
        private string _orderNumber;
        private AppSettings _appSettings;

        [Given(@"I have '([^']*)' quantity '([^']*)', '([^']*)', standard, in my basket")]
        public async Task GivenIaDigitalStandardInMyBasket(int purchaseQuantity, string sapId, string standardTypeToBePurchased)
        {
            _purchaseType = standardTypeToBePurchased.ToLower() switch
            {
                "digital copy" => PurchaseType.DigitalCopy,
                "hard copy" => PurchaseType.HardCopy,
                _ => throw new ArgumentOutOfRangeException(nameof(standardTypeToBePurchased), standardTypeToBePurchased, null)
            };

            _standardSapIdToBePurchased = sapId.Equals("random", StringComparison.OrdinalIgnoreCase) ? _purchaseType.GetRandomStandardSapId() : sapId;

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(_standardSapIdToBePurchased);
            await productPage.AddDisplayedStandardAndProceedToBasket(_purchaseType, purchaseQuantity);

            _standardQuantityPurchased = purchaseQuantity;
        }

        [Given(@"I complete the payment form using '([^']*)' from '([^']*)'")]
        public async Task GivenICompletePaymentUsing(string payBy, string country)
        {
            var user = scenarioContext.Get<UserCredentials>();

            var addressDetails = RandomData.GetAddressDetails(country);

            var cardDetails = payBy.ToLower() switch
            {
                "credit card" => RandomData.GenerateCreditCardDetails(),
                "invoice" => RandomData.GetInvoiceCreditDetails(),
                _ => null
            };

            await basketPage.ProceedToCheckout();
            await paymentInformationPage.FillOutPaymentInformationForm(addressDetails, user.Firstname!, user.Lastname!);

            switch (_purchaseType)
            {
                case PurchaseType.DigitalCopy:
                    await paymentInformationPage.ClickContinueToPayment();
                    break;
                case PurchaseType.HardCopy:
                default:
                    await paymentInformationPage.ClickContinueToShipping();
                    await shippingPage.ClickContinueToPayment();
                    break;
            }

            switch (payBy.ToLower())
            {
                case "invoice":
                    await paymentPage.ClickPayByInvoice();
                    break;
                default:
                    await paymentPage.FillOutPaymentInformationForm(cardDetails!.Number!, cardDetails.Expiry!, cardDetails.CVV!, $"{user.Firstname} {user.Lastname}");
                    break;
            }

            _modeOfPayment = payBy.Equals("credit card", StringComparison.OrdinalIgnoreCase) ? "card" : "invoice";
        }

        [When(@"I click pay now")]
        public async Task WhenICompleteCheckout()
        {
            switch (_modeOfPayment)
            {
                case "invoice":
                    await paymentPage.ClickCompleteOrder();
                    break;
                case "card":
                    await paymentPage.ClickPayNow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_modeOfPayment), _modeOfPayment, null);
            }
        }

        [Then(@"the standard purchase should be successful")]
        public async Task ThenTheDigitalStandardPurchaseShouldBeSuccessful()
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            _orderNumber = await paymentConfirmationPage.GetOrderNumber();
            _orderNumber.Should().NotBeNullOrEmpty();

            // let get order number from order page
            await paymentConfirmationPage.ClickContinueShopping();

            // extract only the order number
            _orderNumber = _orderNumber[(_orderNumber.IndexOf('-') + 1)..];
        }

        [Then(@"purchase details should appear in the dwh database")]
        public async Task ThenPurchaseDetailsShouldAppearInTheDwhDatabase()
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            // sap integration db
            var sapDbResponse = _orderNumber.QueryOrderStateFromSapIntegrationDb(_appSettings, logger);

            // it takes a tick for the data to land in the db, a little annoying
            var count = 0;
            var cancellationToken = new CancellationTokenSource();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            while (count < 5 && sapDbResponse == null)
            {
                await Task.Delay(500, cancellationToken.Token);
                sapDbResponse = _orderNumber.QueryOrderStateFromSapIntegrationDb(_appSettings, logger);
                count++;
            }

            sapDbResponse.Rows.Count.Should().Be(1);
            var sapIntegrationDbResponse = JsonSerializer.Deserialize<SapDwhDbResponseObject>(sapDbResponse.Rows[0]["SapResponse"].ToString()!);

            sapIntegrationDbResponse!.OrderStatus.Should().Be("OrderCreated", $"Purchased standard: {_standardSapIdToBePurchased}, with order number: {_orderNumber} did not make it to the Sap db.");
            sapIntegrationDbResponse.SapItemsData.FirstOrDefault()!.ProductId.Should().Be(_standardSapIdToBePurchased, $"Order did not make it to the Sap db, but order was successful on the front end with order no: {_orderNumber}");
            sapIntegrationDbResponse.SapItemsData.FirstOrDefault()!.Quantity.Should().Be(_standardQuantityPurchased, $"Order quantity did not match what was purchased that is in the Sap db, but order was successful on the front end with order no: {_orderNumber}");

            // dwh db
            var dwhDbResponse = _orderNumber.QueryOrdersFromDwhDb(_appSettings, logger);
            var paymentType = dwhDbResponse.Rows[0]["PaymentType"].ToString();

            dwhDbResponse.Rows.Count.Should().Be(1);
            dwhDbResponse.Rows[0]["Number"].ToString().Should().Be(_orderNumber, $"Order did not make it to the dwh db, but order was successful on the front end with order no: {_orderNumber}");
            paymentType.Should().NotBeNullOrEmpty("PaymentType should not be null or empty");
            paymentType!.ToLower().Should().Be(_modeOfPayment.ToLower(), $"Order with order no: {_orderNumber}, was purchased with {_modeOfPayment} mode of payment, but found {paymentType} in the dwh DB");
        }
    }
}
