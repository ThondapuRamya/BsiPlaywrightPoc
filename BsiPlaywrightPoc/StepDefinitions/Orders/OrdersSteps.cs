using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Orders
{
    [Binding]
    public sealed class OrdersSteps(HomePage homePage, OrderPage orderPage, ProductPage productPage)
    {
        private RequestLoggerHelper? _loggerHelper;
        private string? _standardDesignatorOnOrderHistoryPage;
        private string? _currentPage;

        [Given(@"I am on ([^']*) page")]
        public async Task GivenIAmOnOrderPage(string knowledgePage)
        {
            await homePage.NavigateToYourOrders();
            _standardDesignatorOnOrderHistoryPage = await orderPage.GetStandardDesignatorByIndexAsync(0);

            if (knowledgePage.Equals("product", StringComparison.OrdinalIgnoreCase))
            {
                await orderPage.ClickOrderLinkByIndexAsync();
            }
        }

        [When(@"I download a previously purchased standard ([^']*) page")]
        [When(@"I download a previously purchased standard in my ([^']*) history")]
        public async Task WhenIDownloadAPreviouslyPurchasedStandardInMyOrderHistory(string knowledgePage)
        {
            _loggerHelper = orderPage.StartLoggingDownloadRequests(); // Start logging requests

            switch (knowledgePage.ToLower())
            {
                case "order":
                    _loggerHelper = orderPage.StartLoggingDownloadRequests(); 
                    await orderPage.DownloadOrderByIndexAsync();
                    await orderPage.WaitForLoggerAsync(_loggerHelper);
                    break;

                case "product":
                    _loggerHelper = productPage.StartLoggingDownloadRequests();
                    await productPage.DownloadOrderByIndexAsync();
                    await productPage.WaitForLoggerAsync(_loggerHelper);
                    break;
                default:
                    throw new ArgumentException("Invalid page requested");
            }

            _currentPage = knowledgePage;
        }

        [Then(@"It should link to the correct product Id")]
        public async Task ThenItShouldLinkToTheCorrectProductId()
        {
            var firstRequest = _loggerHelper?.Requests.FirstOrDefault();
            var firstResponse = _loggerHelper?.Responses.FirstOrDefault();

            firstRequest.Should().NotBeNull();
            firstResponse.Should().NotBeNull();
            firstResponse!.Status.Should().Be(200);

            if (_currentPage!.Equals("order", StringComparison.OrdinalIgnoreCase))
            {
                await orderPage.ClickOrderLinkByIndexAsync();
            }

            var actualStandardDesignatorOnProductPage = await productPage.GetStandardDesignatorAsync();
            actualStandardDesignatorOnProductPage.Should().Contain(_standardDesignatorOnOrderHistoryPage);
        }
    }
}
