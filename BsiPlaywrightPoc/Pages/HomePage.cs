using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Pages;

[Binding]
public class HomePage(IPage page)
{
    private ILocator UserAvatarLocator => page.Locator("[data-testid='initials-avatar']");
    private ILocator LogOutBtnLocator => page.GetByRole(AriaRole.Button, new() { Name = "Log out"});
    private ILocator HamburgerLocator => page.Locator("[data-testid='hamburger-menu-icon']");
    private ILocator StandardsButtonLocator => page.GetByRole(AriaRole.Link, new() { Name = "Standards"});
    private ILocator YourOrderButtonLocator => page.GetByRole(AriaRole.Link, new() { Name = "Your Orders" });


    public async Task<bool> IsLoggedInAsync() => await UserAvatarLocator.WaitUntilAvailableAndReturnIsVisibleAsync();

    public async Task ClickUserAvatar() => await UserAvatarLocator.WaitUntilAvailableAndClickAsync();

    public async Task<OrderPage> ClickYourOrders()
    {
        await YourOrderButtonLocator.WaitUntilAvailableAndClickAsync();
        return new OrderPage(page);
    }

    public async Task<OrderPage> NavigateToYourOrders()
    {
        await ClickUserAvatar();
        await ClickYourOrders();
        return new OrderPage(page);
    }

    public async Task LogoutAsync()
    {
        await UserAvatarLocator.WaitUntilAvailableAndClickAsync();
        await LogOutBtnLocator.WaitUntilAvailableAndClickAsync();
    }

    public async Task ClickHamburgerAsync() => await HamburgerLocator.WaitUntilAvailableAndClickAsync();

    public async Task<SearchPage> ClickStandardAsync()
    {
        await StandardsButtonLocator.WaitUntilAvailableAndClickAsync();
        return new SearchPage(page);
    }

    public async Task<SearchPage> NavigateToStandardPageAsync()
    {
        await ClickHamburgerAsync();
        await ClickStandardAsync();
        return new SearchPage(page);
    }
}