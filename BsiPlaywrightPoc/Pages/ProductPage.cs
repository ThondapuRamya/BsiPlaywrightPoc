using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.Enums;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class ProductPage(IPage page) : CommonAcrossPages(page)
    {
        private readonly IPage _page = page;

        private ILocator PersonalPurchaseButtonLocator => _page.Locator("text=Personal Purchase");
        private ILocator BuyBoxTitleLocator => _page.Locator("h3:has-text('is already in your collection')");
        private ILocator PurchaseAgainButtonLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Purchase again" });
        private ILocator AddToBasketButtonLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Add to Basket" });
        private ILocator AddToBasketAwayButtonLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Add to Basket Anyway" });
        private ILocator ProceedToBasketLocator => _page.GetByRole(AriaRole.Link, new() { Name = "Proceed to Basket" });
        private ILocator DigitalOptionStandardLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Digital" });
        private ILocator HardCopyOptionStandardLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Hard Copy" });
        private ILocator QuantityInputLocator => _page.Locator("[data-testid='quantity-input']");
        private ILocator StandardDesignatorLocator => _page.GetByTestId("standard-designator");
        private ILocator DownloadButtonLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Download" });
        private ILocator DrmAlertDialogLocator => _page.GetByRole(AriaRole.Alertdialog, new() { Name = "This document is managed by DRM" });

        public async Task ClickDigitalStandardAsync() => await DigitalOptionStandardLocator.WaitUntilAvailableAndClickAsync();

        public async Task ClickHardCopyStandardAsync() => await HardCopyOptionStandardLocator.WaitUntilAvailableAndClickAsync();

        public async Task AddQuantityAsync(int desiredQuantity)
        {
            var currentQuantity = int.Parse((await QuantityInputLocator.WaitUntilAvailableAndReturnValueAsync("value"))!);

            if (currentQuantity != desiredQuantity)
            {
                await QuantityInputLocator.WaitUntilAvailableAndSendTextAsync($"{desiredQuantity}");
            }
        }

        public async Task ClickAddToBasket()
        {
            var isAddToBasketButtonVisible = await AddToBasketButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            if (!isAddToBasketButtonVisible)
            {
                var isPersonalPurchaseButtonVisible = await PersonalPurchaseButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
                switch (isPersonalPurchaseButtonVisible)
                {
                    case true:
                        await PersonalPurchaseButtonLocator.WaitUntilAvailableAndClickAsync();
                        break;
                    default:
                        await PurchaseAgainButtonLocator.WaitUntilAvailableAndClickAsync();
                        break;
                }
            }
            await AddToBasketButtonLocator.WaitUntilAvailableAndClickAsync();

            // sometimes you might get a pop-up for withdrawn standards, if you really want to purchase them or not
            var isAddToBasketAwayButtonVisible = await AddToBasketAwayButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            if (isAddToBasketAwayButtonVisible)
            {
                await AddToBasketAwayButtonLocator.WaitUntilAvailableAndClickAsync();
            }
        }

        public async Task<BasketPage> ClickProceedToBasket()
        {
            await ProceedToBasketLocator.WaitUntilAvailableAndClickAsync();
            return new BasketPage(_page);
        }

        public async Task<BasketPage> AddDisplayedStandardAndProceedToBasket(PurchaseType purchaseType, int purchaseQuantity)
        {
            var isPurchaseAgainButtonVisible = await IsPurchaseAgainButtonVisibleAsync();
            if (isPurchaseAgainButtonVisible)
            {
                await PurchaseAgainButtonLocator.WaitUntilAvailableAndClickAsync();
            }

            switch (purchaseType)
            {
                case PurchaseType.DigitalCopy:
                    await ClickDigitalStandardAsync();
                    break;
                case PurchaseType.HardCopy:
                    await ClickHardCopyStandardAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(purchaseType), purchaseType, null);
            };

            await AddQuantityAsync(purchaseQuantity);
            await ClickAddToBasket();
            await ClickProceedToBasket();
            return new BasketPage(_page);
        }

        public async Task<bool> IsPurchaseAgainButtonVisibleAsync() => await PurchaseAgainButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

        public async Task<string> GetBuyBoxTitleMessage() => await BuyBoxTitleLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<string> GetStandardDesignatorAsync() => await StandardDesignatorLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task DownloadOrderByIndexAsync(int index = 0)
        {
            await DownloadButtonLocator.Nth(index).WaitUntilAvailableAndClickAsync();
            var isDrmAlertDialogVisible = await DrmAlertDialogLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

            if (isDrmAlertDialogVisible)
            {
                var dialogDownloadButton = DrmAlertDialogLocator.GetByRole(AriaRole.Button, new() { Name = "Download" });
                await dialogDownloadButton.WaitUntilAvailableAndClickAsync();
            }
        }
    }
}
