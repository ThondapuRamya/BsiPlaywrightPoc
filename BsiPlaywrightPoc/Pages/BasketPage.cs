using Microsoft.Playwright;
using BsiPlaywrightPoc.Extensions;

namespace BsiPlaywrightPoc.Pages
{
    public class BasketPage(IPage page) : CommonAcrossPages(page)
    {
        private readonly IPage _page = page;

        private ILocator EmptyBasketFriendlyErrorMessageLocator => _page.Locator("[data-testid='empty-basket-title']");
        private ILocator AgreeToTermsCheckboxLocator => _page.Locator("[id='agreeToTerms']");
        private ILocator ProceedToCheckoutLocator => _page.Locator("text=Proceed to Checkout");
        private ILocator ContinueToPaymentLocator => _page.Locator("text=Continue to payment");
        private ILocator ProductIconLocator => _page.GetByRole(AriaRole.Link, new() { Name = "Product Icon" });

        public async Task ClickAgreeToTermsCheckbox() => await AgreeToTermsCheckboxLocator.WaitUntilAvailableAndClickAsync();

        public async Task<int> GetDisplayedProductCount() => await ProductIconLocator.WaitUntilAvailableAndReturnElementsCountAsync();

        public async Task<string> GetEmptyBasketFriendlyErrorMessage() => await EmptyBasketFriendlyErrorMessageLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<PaymentInformationPage> ClickProceedToCheckoutButton()
        {
            await ProceedToCheckoutLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentInformationPage(_page);
        }

        public async Task<PaymentInformationPage> ProceedToCheckout()
        {
            await ClickAgreeToTermsCheckbox();
            await ClickProceedToCheckoutButton();
            return new PaymentInformationPage(_page);
        }
    }
}
