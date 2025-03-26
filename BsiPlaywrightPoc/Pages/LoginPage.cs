using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Pages;

[Binding]
public class LoginPage(IPage page)
{
    private ILocator LoginButtonLocator => page.Locator("[href*='/login?redirectTo']");
    private ILocator EmailFieldLocator => page.Locator("input[name='email']");
    private ILocator PasswordFieldLocator => page.Locator("input[name='password']");
    private ILocator PersonalLogInBtnLocator => page.Locator("button[type='submit']");
    private ILocator LoginFriendlyErrorMessageLocator => page.Locator("[aria-label='Login Failed']");

    public async Task ClickLoginButton()
    {
        await LoginButtonLocator.WaitUntilAvailableAndClickAsync();
    }

    public async Task FillOutLoginForm(string email, string password)
    {
        await EmailFieldLocator.WaitUntilAvailableAndSendTextAsync(email);
        await PasswordFieldLocator.WaitUntilAvailableAndSendTextAsync(password);
    }

    public async Task<HomePage> ClickPersonalLogin()
    {
        await PersonalLogInBtnLocator.WaitUntilAvailableAndClickAsync();
        return new HomePage(page);
    }

    public async Task<HomePage> Login(string email, string password)
    {
        await ClickLoginButton();
        await FillOutLoginForm(email, password);
        await ClickPersonalLogin();
        return new HomePage(page);
    }

    public async Task<string> GetFriendlyErrorMessage()
    {
        return await LoginFriendlyErrorMessageLocator.WaitUntilAvailableAndReturnTextAsync();
    }

    public async Task<bool> IsLoginButtonVisible()
    {
        return await LoginButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
    }
}