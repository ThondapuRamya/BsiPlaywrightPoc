using Microsoft.Playwright;
using BsiPlaywrightPoc.Extensions;

namespace BsiPlaywrightPoc.Pages
{
    public class AccountPage(IPage page)
    {
        private ILocator EditAddressLocator => page.Locator("[href='account/address']");

        public async Task<AddressesPage> ClickEditAddress()
        {
            await EditAddressLocator.WaitUntilAvailableAndClickAsync();
            return new AddressesPage(page);
        }
    }
}
