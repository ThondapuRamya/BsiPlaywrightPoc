using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class OrderPage(IPage page) : CommonAcrossPages(page)
    {
        private readonly IPage _page = page;

        private ILocator OrderNumberLocator => _page.Locator("text=Order Number").First.Locator("..").Last;
        private ILocator DownloadButtonLocator => _page.Locator("[aria-label='Download']");
        private ILocator DrmAlertDialogLocator => _page.GetByRole(AriaRole.Alertdialog, new() { Name = "This document is managed by DRM" });
        private ILocator StandardLinksLocator => _page.GetByTestId("order-line-product-link");
        private ILocator StandardDesignatorLocator => _page.GetByTestId("order-line-standard-designator");

        public async Task<string> GetFirstOrderNumberDisplayed() => await OrderNumberLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<string> GetOrderNumberByTitle(string standardTitle)
        {
            var productLinkLocator = _page.Locator($"button[data-testid='order-line-product-link']:has-text('{standardTitle}')");
            var orderContainer = productLinkLocator.Locator("ancestor::div[role='group']");
            var orderNumberLocator = orderContainer.Locator("text=Order Number").First.Locator("..").Last;

            return await orderNumberLocator.WaitUntilAvailableAndReturnTextAsync();
        }

        public async Task<string> GetStandardDesignatorByIndexAsync(int index) => await StandardDesignatorLocator.Nth(index).WaitUntilAvailableAndReturnTextAsync();
        
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

        public async Task ClickOrderLinkByIndexAsync(int index = 0) => await StandardLinksLocator.Nth(index).WaitUntilAvailableAndClickAsync();
    }
}
