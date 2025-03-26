using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.User;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentInformationPage(IPage page)
    {
        private ILocator CountryDropdownLocator => page.GetByRole(AriaRole.Combobox, new() { Name = "Country/region" });
        private ILocator StateOrTerritoryDropdownLocator => page.GetByRole(AriaRole.Combobox, new() { Name = "State/territory" });
        private ILocator RegionDropdownLocator => page.Locator("[placeholder='Region']");
        private ILocator ProvinceDropdownLocator => page.GetByRole(AriaRole.Combobox, new() { Name = "Province" });
        private ILocator CountyDropdownLocator => page.GetByRole(AriaRole.Combobox, new() { Name = "County" });
        private ILocator FirstnameInputFieldLocator => page.GetByRole(AriaRole.Textbox, new() { Name = "First name" });
        private ILocator LastnameInputFieldLocator => page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" });
        private ILocator AddressInputFieldLocator => page.Locator("[placeholder='Address'], [placeholder='Street and house number']");
        private ILocator CityInputFieldLocator => page.GetByRole(AriaRole.Textbox, new() { Name = "City" });
        private ILocator SuburbInputFieldLocator => page.GetByRole(AriaRole.Textbox, new() { Name = "Suburb" });
        private ILocator PostcodeInputFieldLocator => page.Locator("[placeholder='Postcode'], [placeholder='Postal code']");
        private ILocator ContinueToPaymentButtonLocator => page.GetByRole(AriaRole.Button, new() { Name = "Continue to payment" });
        private ILocator ContinueToShippingButtonLocator => page.Locator("text=Continue to shipping");
        private ILocator AddressSuggestionCloseIconLocator => page.Locator("[aria-label='Close suggestions']");
        private ILocator OneTrustCookieAcceptButtonLocator => page.Locator("#onetrust-accept-btn-handler");

        public async Task FillOutPaymentInformationForm(UserAddressDetails userAddressDetails, string firstname, string lastname)
        {
            // Wait for the cookie consent button to appear and click it
            var isCookieAcceptButton = await OneTrustCookieAcceptButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            if (isCookieAcceptButton)
            {
                await OneTrustCookieAcceptButtonLocator.ClickAsync();
            }

            await CountryDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Country);
            await FirstnameInputFieldLocator.WaitUntilAvailableAndSendTextAsync(firstname);
            await LastnameInputFieldLocator.WaitUntilAvailableAndSendTextAsync(lastname);
            await AddressInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.Address!);

            var isAddressSuggestionVisible = await AddressSuggestionCloseIconLocator.WaitUntilAvailableAndReturnIsVisibleAsync(2000);
            if (isAddressSuggestionVisible)
            {
                await AddressSuggestionCloseIconLocator.WaitUntilAvailableAndClickAsync();
            }

            if (userAddressDetails.Country.ToLower() != "australia")
            {
                await CityInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.City!);
            }

            if (userAddressDetails.Country.ToLower() != "zimbabwe")
            {
                await PostcodeInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.Postcode!);
            }

            switch (userAddressDetails.Country.ToLower())
            {
                case "australia":
                    await SuburbInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.Suburb!);
                    await StateOrTerritoryDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.State!);
                    break;

                case "new zealand":
                    await RegionDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Region!);
                    break;

                case "canada":
                    await ProvinceDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Province!);
                    break;

                case "ireland":
                    await CountyDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.County!);
                    break;
            }
        }

        public async Task<PaymentPage> ClickContinueToPayment()
        {
            await ContinueToPaymentButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentPage(page);
        }

        public async Task<ShippingPage> ClickContinueToShipping()
        {
            await ContinueToShippingButtonLocator.WaitUntilAvailableAndClickAsync();
            return new ShippingPage(page);
        }
    }
}
