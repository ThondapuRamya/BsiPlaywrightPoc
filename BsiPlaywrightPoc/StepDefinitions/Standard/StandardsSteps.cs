using BsiPlaywrightPoc.Pages;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Standard
{
    [Binding]
    public sealed class StandardsSteps(HomePage homePage, SearchPage searchPage)
    {
        [Given(@"I navigate to knowledge standard page")]
        public async Task GivenIAmOnTheKnowledgeStandardPage()
        {
            await homePage.ClickHamburgerAsync();
            await homePage.ClickStandardAsync();
        }

        [Then(@"(.*) standards count should be displayed")]
        public async Task ThenSeveralStandardsShouldBeVisible(int expectedStandardCount)
        {
            var actualStandardCount = await searchPage.GetDisplayedStandardsCountAsync();
            actualStandardCount.Should().Be(expectedStandardCount);
        }
    }
}
