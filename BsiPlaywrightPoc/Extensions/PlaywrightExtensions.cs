using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Extensions;

public static class PlaywrightExtensions
{
    private const int DefaultTimeout = 10000; // Default timeout of 10 seconds

    public static async Task WaitUntilAvailableAndClickAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        await locator.First.ClickAsync();
    }

    public static async Task WaitUntilAvailableAndSendTextAsync(this ILocator locator, string text, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        await WaitUntilAvailableAndClearExistingValueAsync(locator);
        await locator.First.FillAsync(text);
    }

    public static async Task WaitUntilAvailableAndClearExistingValueAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        await locator.First.FillAsync(string.Empty);
    }

    public static async Task<string> WaitUntilAvailableAndReturnTextAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        return await locator.First.InnerTextAsync();
    }

    public static async Task<bool> WaitUntilAvailableAndReturnIsVisibleAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        try
        {
            await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
            return await locator.First.IsVisibleAsync();
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public static async Task<string?> WaitUntilAvailableAndReturnValueAsync(this ILocator locator, string attributeName, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        return await locator.GetAttributeAsync(attributeName);
    }

    public static async Task WaitUntilAvailableAndSelectFromDropdownByTextAsync(this ILocator locator, string text, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        await locator.First.SelectOptionAsync(new SelectOptionValue { Label = text });
    }
    
    public static async Task<int> WaitUntilAvailableAndReturnElementsCountAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        return await locator.CountAsync();
    }
    
    public static async Task<IList<string>> WaitUntilAvailableAndExtractAllTextsAsync(this ILocator locator, int timeout = DefaultTimeout)
    {
        await locator.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeout });
        var count = await locator.CountAsync();
        var texts = new List<string>();
        for (var i = 0; i < count; i++)
        {
            texts.Add(await locator.Nth(i).InnerTextAsync());
        }
        return texts;
    }
}