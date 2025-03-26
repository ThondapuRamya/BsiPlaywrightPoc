using BsiPlaywrightPoc.Model.AppSettings;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using BrowserType = BsiPlaywrightPoc.Model.Enums.BrowserType;
using Microsoft.Extensions.Logging;

namespace BsiPlaywrightPoc.Web
{
    public class PlaywrightDriver : IAsyncDisposable
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly Lazy<Task<IPage>> _page;
        private readonly BrowserType _browserType;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private AppSettings? _appSettings;
        private readonly ILogger _logger;

        public PlaywrightDriver(ScenarioContext scenarioContext, BrowserType browserType, ILogger logger)
        {
            _scenarioContext = scenarioContext;
            _browserType = browserType;
            _logger = logger;
            _page = new Lazy<Task<IPage>>(InitialisePlaywright); // Lazy initialization
        }

        public async Task<IPage> GetPageAsync() => await _page.Value;

        private async Task<IPage> InitialisePlaywright()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();
            var playwright = await Playwright.CreateAsync();

            var browser = _browserType switch
            {
                BrowserType.Firefox => playwright.Firefox,
                BrowserType.WebKit => playwright.Webkit,
                _ => playwright.Chromium,
            };

            _browser = await browser.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = _appSettings.RuntimeSettings.LunchBrowserInHeadlessMode,
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                HttpCredentials = new HttpCredentials
                {
                    Username = _appSettings.KnowledgeBasicAuth.UserName!,
                    Password = _appSettings.KnowledgeBasicAuth.Password!
                }
            });

            IPage page = null!;
            int retryCount = 3;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    page = await _context.NewPageAsync();
                    await page.GotoAsync(_appSettings.BsiKnowledge.HostName!, new PageGotoOptions { Timeout = 60000 });

                    // Handle cookie consent
                    var cookieAcceptButton = await page.WaitForSelectorAsync("#onetrust-accept-btn-handler", new PageWaitForSelectorOptions { Timeout = 10000 });
                    if (cookieAcceptButton != null)
                    {
                        await cookieAcceptButton.ClickAsync();
                    }

                    break; // Break loop if successful
                }
                catch (TimeoutException ex)
                {
                    _logger.LogError($"Timeout while navigating to {_appSettings.BsiKnowledge.HostName}. Attempt {i + 1}/{retryCount}. Error: {ex.Message}");

                    if (i == retryCount - 1) throw; // If last attempt, throw exception
                }
            }

            return page;
        }

        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }
    }
}
