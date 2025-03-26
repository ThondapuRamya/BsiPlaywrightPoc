using Microsoft.Playwright;
using BsiPlaywrightPoc.Extensions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Pages;

[Binding]
public class SearchPage(IPage page)
{
    private ILocator StandardsListLocator => page.Locator("[data-testid='representing-designator']");
    private ILocator SearchFieldLocator => page.GetByRole(AriaRole.Searchbox, new() { Name = "Search for keyword, standard name or code" });
    private ILocator DisplayedStandards => page.Locator("[data-testid='product-list-title']");

    public async Task<int> GetDisplayedStandardsCountAsync() => await StandardsListLocator.WaitUntilAvailableAndReturnElementsCountAsync();

    public async Task SearchForStandardAsync(string sapId)
    {
        await SearchFieldLocator.WaitUntilAvailableAndSendTextAsync(sapId);
        var standardDisplayedCount = await DisplayedStandards.CountAsync();

        var maxWaitTime = TimeSpan.FromSeconds(10); // Maximum time to wait
        var startTime = DateTime.UtcNow;

        var cancellationToken = new CancellationTokenSource();
        while (standardDisplayedCount > 1)
        {
            if (DateTime.UtcNow - startTime > maxWaitTime)
            {
                break; // Exit the loop if it exceeds the maximum wait time
            }

            await Task.Delay(1000, cancellationToken.Token);
            standardDisplayedCount = await DisplayedStandards.CountAsync();
        }
        await Task.Delay(1000, cancellationToken.Token);
    }

    public async Task<string> GetDisplayedStandardsTitleAsync() => await DisplayedStandards.WaitUntilAvailableAndReturnTextAsync();

    public async Task OpenDisplayedStandardAsync() => await DisplayedStandards.WaitUntilAvailableAndClickAsync();

    public async Task<ProductPage> SearchAndOpenStandard(string sapId)
    {
        await SearchForStandardAsync(sapId);
        await OpenDisplayedStandardAsync();

        return new ProductPage(page);
    }
}