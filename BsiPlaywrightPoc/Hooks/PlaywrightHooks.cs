using BoDi;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Web;
using TechTalk.SpecFlow;
using Microsoft.Playwright;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow.Infrastructure;
using BrowserType = BsiPlaywrightPoc.Model.Enums.BrowserType;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class PlaywrightHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IObjectContainer _container;
        private readonly ISpecFlowOutputHelper _specFlowOutputHelper;
        private AppSettings _appSettings;
        private readonly ILogger _logger;
        private PlaywrightDriver? _driver;

        public PlaywrightHooks(
            ScenarioContext scenarioContext,
            IObjectContainer container,
            ISpecFlowOutputHelper specFlowOutputHelper,
            AppSettings appSettings,
            ILogger logger)
        {
            _scenarioContext = scenarioContext;
            _container = container;
            _specFlowOutputHelper = specFlowOutputHelper;
            _logger = logger;
        }

        [BeforeScenario("@WebScenario", Order = HookOrdering.WebSetting)]
        public async Task BeforeScenarioRegisterPlaywrightDriver()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();
            var browserType = _appSettings.RuntimeSettings.BrowserType switch
            {
                "Firefox" => BrowserType.Firefox,
                "Webkit" => BrowserType.WebKit,
                _ => BrowserType.Chromium
            };

            // Register the PlaywrightDriver in the container
            _driver = new PlaywrightDriver(_scenarioContext, browserType, _logger);
            _container.RegisterInstanceAs(_driver);
            _container.RegisterInstanceAs(await _driver.GetPageAsync());
        }

        [AfterStep("@WebScenario", Order = HookOrdering.ScreenShot)]
        public async Task AfterWebStep()
        {
            if (_scenarioContext.TestError != null)
            {
                var page = await _driver!.GetPageAsync();
                var screenShotPath = Path.Combine("Screenshots", $"{_scenarioContext.ScenarioInfo.Title.Replace(" ", "_")}.png");
                Directory.CreateDirectory("Screenshots"); // Ensure the directory exists
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenShotPath });

                _specFlowOutputHelper.AddAttachment(screenShotPath);
                _specFlowOutputHelper.WriteLine($"ScreenShot saved to: {Path.GetFullPath(screenShotPath)}");
            }
        }

        [AfterScenario("@WebScenario")]
        public async Task AfterTestScenario()
        {
            if (_driver != null)
            {
                await _driver.DisposeAsync();
                _driver = null;
            }
        }
    }
}
