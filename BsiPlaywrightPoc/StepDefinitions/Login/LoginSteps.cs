using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Login;

[Binding]
public class LoginSteps(LoginPage loginPage, HomePage homePage)
{
    [Given(@"I navigate to knowledge log in page")]
    public async Task GivenIAmOnTheKnowledgeLogInPage()
    {
        await loginPage.ClickLoginButton();
    }

    [Given(@"I fillOut username '([^']*)' and password '([^']*)' credentials")]
    public async Task GivenIFillOutUsernameAndPasswordCredentials(string userName, string password)
    {
        await loginPage.FillOutLoginForm(userName, password);
    }

    [Given(@"I am logged In")]
    public async Task GivenIAmLoggedIn()
    {
        await loginPage.FillOutLoginForm("test.test@example.org", "Password123");
        await loginPage.ClickPersonalLogin();
    }

    [When(@"I click login")]
    public async Task WhenIClickLogin()
    {
        await loginPage.ClickPersonalLogin();
    }

    [Then(@"the user login should be '([^']*)'")]
    public async Task ThenTheUserLoginShouldBe(string expectedResults)
    {
        if (expectedResults.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
        {
            var friendlyErrorMessage = await loginPage.GetFriendlyErrorMessage();
            friendlyErrorMessage.Should().Be(expectedResults);
        }
        else
        {
            var isUserLoggedIn = await homePage.IsLoggedInAsync();
            isUserLoggedIn.Should().BeTrue();
        }
    }

    [When(@"I click logout")]
    public async Task WhenIClickLogout()
    {
        await homePage.LogoutAsync();
    }

    [Then(@"I should be logged out successfully")]
    public async Task ThenIShouldBeLoggedOutSuccessfully()
    {
        var isLoginButtonVisible = await loginPage.IsLoginButtonVisible();
        isLoginButtonVisible.Should().BeTrue();
    }
}