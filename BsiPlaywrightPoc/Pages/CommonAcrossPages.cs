using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Helpers;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class CommonAcrossPages(IPage page)
    {
        private ILocator BasketIconLocator => page.Locator("[data-testid='basket-link']");

        public async Task ClickBasketIcon() => await BasketIconLocator.WaitUntilAvailableAndClickAsync();

        public RequestLoggerHelper StartLoggingDownloadRequests()
        {
            var loggerHelper = new RequestLoggerHelper("auth/download/standard");
            loggerHelper.StartLogging(page); // Start logging requests

            return loggerHelper;
        }

        public async Task WaitForLoggerAsync(RequestLoggerHelper logger) =>
            await page.WaitForResponseAsync(response =>
                logger.Responses.Any(r => r.Url == response.Url && r.Status == 200));
    }
}
