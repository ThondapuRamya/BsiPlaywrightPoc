using BsiPlaywrightPoc.Model.Enums;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Basket
{
    [Binding]
    public sealed class BasketSteps(
        ScenarioContext scenarioContext,
        LoginPage loginPage,
        HomePage homePage,
        SearchPage searchPage,
        ProductPage productPage,
        BasketPage basketPage)
    {
        private PurchaseType _purchaseType;
        private string? _initialStandardToBeAddedToBasket;
        private string? _differentStandardToBeAddedToBasket;

        [Given(@"I have a standard in my basket while signed in")]
        public async Task GivenIHaveAStandardInMyBasketWhileSignedIn()
        {
            _purchaseType = PurchaseType.DigitalCopy;
            _initialStandardToBeAddedToBasket = _purchaseType.GetRandomStandardSapId();

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(_initialStandardToBeAddedToBasket);
            await productPage.AddDisplayedStandardAndProceedToBasket(_purchaseType, 1);
        }

        [Given(@"Given I have added another standard to the basket while signed out")]
        public async Task GivenGivenIHaveAddedAnotherStandardToTheBasketWhileSignedOut()
        {
            await homePage.LogoutAsync();
            _differentStandardToBeAddedToBasket = _purchaseType.GetRandomStandardSapId();

            var counter = 0;
            while (_differentStandardToBeAddedToBasket == _initialStandardToBeAddedToBasket && counter <= 5)
            {
                _differentStandardToBeAddedToBasket = _purchaseType.GetRandomStandardSapId();
                counter++;
            }

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(_differentStandardToBeAddedToBasket);
            await productPage.AddDisplayedStandardAndProceedToBasket(_purchaseType, 1);
        }

        [When(@"I sign back in")]
        public async Task WhenISignBackIn()
        {
            var user = scenarioContext.Get<UserCredentials>();
            await loginPage.Login(user.Email!, user.Password!);
        }

        [Then(@"Both standards should be visible in my basket")]
        public async Task ThenBothStandardsShouldBeVisibleInMyBasket()
        {
            var cancellationToken = new CancellationTokenSource();
            await Task.Delay(5000, cancellationToken.Token);

            var actualDisplayedProductCount = await basketPage.GetDisplayedProductCount();
            actualDisplayedProductCount.Should().Be(2);
        }

        [Given(@"I navigate to the basket page")]
        public async Task GivenIAmOnTheBasketBasketPage()
        {
            await basketPage.ClickBasketIcon();
        }

        [Then(@"([^']*) should be visible")]
        public async Task ThenThereAreNoItemsInYourBasketShouldBeVisible(string expectedBasketFriendlyErrorMessage)
        {
            var actualBasketFriendlyErrorMessage = await basketPage.GetEmptyBasketFriendlyErrorMessage();
            actualBasketFriendlyErrorMessage.Should().Contain(expectedBasketFriendlyErrorMessage);
        }

        [Given(@"I search and open a previously purchased standard")]
        public async Task GivenISearchAndOpenAPreviouslyPurchasedStandard()
        {
            var purchasedStandardSapId = scenarioContext.Get<string>("PurchasedSapId");

            await homePage.NavigateToStandardPageAsync();
            await searchPage.SearchAndOpenStandard(purchasedStandardSapId);
        }

        [Then(@"Add to basket button should be hidden")]
        public async Task ThenAddToBasketButtonShouldBeHidden()
        {
            var isPurchaseAgainButtonVisible = await productPage.IsPurchaseAgainButtonVisibleAsync();
            isPurchaseAgainButtonVisible.Should().BeTrue();
        }

        [Then(@"([^']*) as a friendly message should also be visible")]
        public async Task ThenThisStandardIsAlreadyInYourCollectionAsAFriendlyMessageShouldAlsoBeVisible(string expectedBuyBoxTitleMessage)
        {
            var actualBuyBoxTitleMessage = await productPage.GetBuyBoxTitleMessage();
            actualBuyBoxTitleMessage.Should().Be(expectedBuyBoxTitleMessage);
        }
    }
}
