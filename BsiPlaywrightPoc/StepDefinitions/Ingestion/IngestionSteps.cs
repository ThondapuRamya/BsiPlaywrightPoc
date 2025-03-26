using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.StepDefinitions.Ingestion;

[Binding]
public sealed class IngestionSteps(
    ScenarioContext scenarioContext,
    SearchPage searchPage,
    IngestionHelper ingestionHelper,
    ILogger logger)
{
    private Model.Standard? _standard;
    private AppSettings? _appSettings;

    [Given(@"I have ingested a digital copy")]
    public async Task GivenIHaveIngestedADigitalCopy()
    {
        _appSettings = scenarioContext.Get<AppSettings>();

        _standard = "DigitalOnlyStandard".GetStandardByName();
        var standardDbResponse = _standard?.SapId.QueryStandardDb(_appSettings, logger);

        if (standardDbResponse?.Rows != null)
        {
            var response = await ingestionHelper.DeleteIngestedStandard(_standard!);
            response!.DeletedSapIds!.FirstOrDefault().Should().Be(_standard?.SapId);
        }

        await ingestionHelper.IngestStandard(_standard!);
    }

    [When(@"I search for the ingested standard")]
    public async Task WhenISearchForTheIngestedStandard()
    {
        var standardDbResponse = _standard?.SapId.QueryStandardDb(_appSettings!, logger);
        standardDbResponse?.Rows.Count.Should().Be(1);

        await searchPage.SearchForStandardAsync(_standard?.SapId!);
    }

    [Then(@"it should be visible")]
    public async Task ThenItShouldBeVisible()
    {
        var actualTitle = await searchPage.GetDisplayedStandardsTitleAsync();
        actualTitle.Should().Be(_standard?.Title);
    }
}