using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class TaxPage(IPage page)
    {
        private ILocator SaveButtonLocator => page.GetByRole(AriaRole.Button, new() { Name = "Save VAT number" });
        private ILocator VatInputFieldLocator => page.Locator("#vatNumber");
        private ILocator ReturnToAccountPageLocator => page.GetByRole(AriaRole.Link, new() { Name = " Return to Your Account" });
        private ILocator CodiceFiscaleTextFieldLocator => page.GetByRole(AriaRole.Textbox, new() { Name = "Codice fiscale (Italian addresses only)" });

        public async Task EditVatNumber(string vatNumber, string vatType)
        {
            await SelectVatType(vatType);
            await VatInputFieldLocator.WaitUntilAvailableAndSendTextAsync(vatNumber);
            await SaveButtonLocator.WaitUntilAvailableAndClickAsync();
        }

        public async Task EditCodiceFiscale(string codiceFiscale) => await CodiceFiscaleTextFieldLocator.WaitUntilAvailableAndSendTextAsync(codiceFiscale);

        private async Task SelectVatType(string type)
        {
            switch (type.ToLower())
            {
                case "uk":
                    await page.GetByRole(AriaRole.Radio, new() { Name = "UK VAT Number" }).ClickAsync();
                    break;
                case "eu":
                    await page.GetByRole(AriaRole.Radio, new() { Name = "EU VAT Number" }).ClickAsync();
                    break;
                case "other":
                    await page.GetByRole(AriaRole.Radio, new() { Name = "Other" }).ClickAsync();
                    break;
                default:
                    throw new ArgumentException("Invalid vat type");
            }
        }
        
        public async Task<AccountPage> ClickReturnToYourAccount()
        {
            await ReturnToAccountPageLocator.WaitUntilAvailableAndClickAsync();
            return new AccountPage(page);
        }
    }
}
