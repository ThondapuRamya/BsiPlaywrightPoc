using Microsoft.Playwright;
using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.User;

namespace BsiPlaywrightPoc.Pages
{
    public class AddressesPage(IPage page)
    {
        private ILocator AddressInputFieldLocator => page.Locator("#edit-addresses-defaultAddress-address1");
        private ILocator CityInputFieldLocator => page.Locator("#edit-addresses-defaultAddress-city");
        private ILocator ZipInputFieldLocator => page.Locator("#edit-addresses-defaultAddress-zip");
        private ILocator PhoneInputFieldLocator => page.Locator("#edit-addresses-defaultAddress-phone");
        private ILocator CountryDropdownFieldLocation => page.Locator("text=Country/Region");
        private ILocator SaveButtonLocator => page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" });

        public async Task EditDefaultAddress(UserAddressDetails address)
        {
            await AddressInputFieldLocator.WaitUntilAvailableAndSendTextAsync(address.Address!);
            await CityInputFieldLocator.WaitUntilAvailableAndSendTextAsync(address.City!);
            await ZipInputFieldLocator.WaitUntilAvailableAndSendTextAsync(address.Postcode!);
            await PhoneInputFieldLocator.WaitUntilAvailableAndSendTextAsync(address.Phone!);

            await CountryDropdownFieldLocation.WaitUntilAvailableAndClickAsync();
            var countryOptionLocator = page.GetByRole(AriaRole.Option, new() { Name = $"{address.Country}" });
            await countryOptionLocator.WaitUntilAvailableAndClickAsync();

            await SaveButtonLocator.WaitUntilAvailableAndClickAsync();
        }
    }
}
