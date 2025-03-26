using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentConfirmationPage(IPage page)
    {
        private ILocator OrderConfirmationNumberLocator => page.Locator("[class='os-order-number']");
        private ILocator ContinueShoppingButtonLocator => page.Locator("text=Continue shopping");

        public async Task<string> GetOrderNumber()
        {
            var orderNumber = await OrderConfirmationNumberLocator.WaitUntilAvailableAndReturnTextAsync(20000);
            return orderNumber[(orderNumber.IndexOf('-') + 1)..]; // extract the number only
        }

        public async Task<HomePage> ClickContinueShopping()
        {
            await ContinueShoppingButtonLocator.WaitUntilAvailableAndClickAsync();
            return new HomePage(page);
        }
    }
}
