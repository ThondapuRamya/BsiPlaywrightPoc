using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentPage(IPage page)
    {
        // Locators targeting the iframes and the input fields within them
        // Define locators for the iframes
        private IFrameLocator CardNumberIframeLocator => page.FrameLocator("iframe[id^='card-fields-number']");
        private IFrameLocator ExpiryDateIframeLocator => page.FrameLocator("iframe[id^='card-fields-expiry']");
        private IFrameLocator SecurityCodeIframeLocator => page.FrameLocator("iframe[id^='card-fields-verification_value']");
        private IFrameLocator NameOnCardIframeLocator => page.FrameLocator("iframe[id^='card-fields-name']");

        // Define locators for the input fields within the iframes
        private ILocator CardNumberInputFieldLocator => CardNumberIframeLocator.Locator("input");
        private ILocator ExpiryDateInputFieldLocator => ExpiryDateIframeLocator.Locator("text=Expiration date (MM / YY)");
        private ILocator SecurityCodeInputFieldLocator => SecurityCodeIframeLocator.Locator("text=Security code");
        private ILocator NameOnCardInputFieldLocator => NameOnCardIframeLocator.Locator("text=Name on card");
        private ILocator PayNowButtonLocator => page.GetByRole(AriaRole.Button, new() { Name = "Pay now" });

        // pay by invoice
        private ILocator PayByInvoiceRadioButtonLocator => page.Locator("text=Pay by Invoice");
        private ILocator CompleteOrderButtonLocator => page.Locator("text=Complete order");
        private ILocator ProfilePageLinkLocator => page.GetByRole(AriaRole.Link, new() { Name = "profile page" });
        private ILocator ChangeVatLocator => page.GetByRole(AriaRole.Link, new() { Name = "Change VAT Number" });
        private ILocator DisabledPayNowButtonLocator => page.Locator("#continue_button.btn--disabled");
        private ILocator SubTotalPriceLocator => page.Locator("[class*='total-line total-line--subtotal']");
        private ILocator EstimatedTaxesLocator => page.Locator("[class*='total-line total-line--taxes']");
        private ILocator TotalPriceLocator => page.Locator("[class*='total-line-table__footer']");
        private ILocator VatNumberFriendMessageLocator => page.Locator("[class*='section__text--tax']");
        private ILocator ItalianUserFriendErrorMessageLocator => page.Locator("[class*='old-bsi section__text--tax']");


        public async Task FillOutPaymentInformationForm(string cardNumber, string cardExpiryDate, string cardSecurityCode, string nameOnPaymentCard)
        {
            // Fill out the Name on Card
            await NameOnCardInputFieldLocator.WaitUntilAvailableAndSendTextAsync(nameOnPaymentCard);
            await SecurityCodeInputFieldLocator.WaitUntilAvailableAndSendTextAsync(cardSecurityCode);
            await ExpiryDateInputFieldLocator.WaitUntilAvailableAndSendTextAsync(cardExpiryDate);
            await CardNumberInputFieldLocator.First.WaitUntilAvailableAndSendTextAsync(cardNumber);
        }

        public async Task ClickPayByInvoice() => await PayByInvoiceRadioButtonLocator.WaitUntilAvailableAndClickAsync();

        public async Task<PaymentConfirmationPage> ClickPayNow()
        {
            await PayNowButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentConfirmationPage(page);
        }

        public async Task<PaymentConfirmationPage> ClickCompleteOrder()
        {
            await CompleteOrderButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentConfirmationPage(page);
        }

        public async Task<bool> IsPayNowButtonVisible() =>
             await PayNowButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

        public async Task<bool> IsVatNumberVisible(string vatNumber)
        {
            var locator = page.GetByRole(AriaRole.Cell, new() { Name = $"{vatNumber}" });
            return await locator.WaitUntilAvailableAndReturnIsVisibleAsync();
        }

        public async Task<TaxPage> ClickChangeVatNumber()
        {
            await ChangeVatLocator.WaitUntilAvailableAndClickAsync();
            return new TaxPage(page);
        }

        public async Task<string> GetTotalPrice()
        {
            var price = await TotalPriceLocator.WaitUntilAvailableAndReturnTextAsync();
            return price[(price.IndexOf('£') + 1)..];
        }

        public async Task<string> GetSubTotalPrice()
        {
            var price = await SubTotalPriceLocator.WaitUntilAvailableAndReturnTextAsync();
            return price[(price.IndexOf('£') + 1)..];
        }

        public async Task<string> GetEstimatedTaxes()
        {
            var price = await EstimatedTaxesLocator.WaitUntilAvailableAndReturnTextAsync();
            return price[(price.IndexOf('£') + 1)..];
        }

        public async Task<string> GetVatFriendlyMessage() => await VatNumberFriendMessageLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<string> GetItalianVatFriendlyMessage() => await ItalianUserFriendErrorMessageLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<bool> IsPayNowButtonDisabled() =>
            await DisabledPayNowButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

        public async Task<bool> IsEstimatedTaxesVisible() =>
            await EstimatedTaxesLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

        public async Task<bool> IsSubTotalPriceVisible() =>
            await SubTotalPriceLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
    }
}
