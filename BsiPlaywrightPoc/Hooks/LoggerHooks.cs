using BoDi;
using BsiPlaywrightPoc.Factory.AppSettings;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public sealed class LoggerHooks(ScenarioContext scenarioContext, IObjectContainer container)
    {
        private ILogger? _logger;

        [BeforeScenario(Order = HookOrdering.BetweenFeatureAndScenarioSetting)]
        public void BeforeScenarioCreateLogger()
        {
            // Set up logging
            var appSettingsFactory = new AppSettingsFactory();
            _logger = LoggerFactory.Create(config =>
            {
                config.AddConfiguration(appSettingsFactory.GetLoggingConfiguration());
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Information);
            }).CreateLogger(GetType());

            if (_logger == null)
            {
                throw new Exception("Logger was not initialized correctly.");
            }

            _logger.LogInformation($"Scenario '{scenarioContext.ScenarioInfo.Title}' is starting.");

            // register the logger so it can be injected
            container.RegisterInstanceAs(_logger);
        }

        [AfterScenario]
        public void AfterScenarioDisposeLogger()
        {
            var disposableLogger = _logger as IDisposable;
            disposableLogger?.Dispose();
        }
    }
}
