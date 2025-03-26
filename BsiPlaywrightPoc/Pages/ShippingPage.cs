using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class ShippingPage(IPage page)
    {
        private ILocator ContinueToPaymentButtonLocator => page.Locator("text=Continue to payment");

        public async Task<PaymentPage> ClickContinueToPayment()
        {
            await ContinueToPaymentButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentPage(page);
        }
    }
}
