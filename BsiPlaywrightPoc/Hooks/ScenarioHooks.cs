using TechTalk.SpecFlow;
using BsiPlaywrightPoc.Factory.AppSettings;
using BsiPlaywrightPoc.Model.AppSettings;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class ScenarioHooks(FeatureContext featureContext, ScenarioContext scenarioContext)
    {
        private readonly FeatureContext _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        private static AppSettings? _appSettings;

        [BeforeTestRun(Order = HookOrdering.BeforeAndAfterSetting)]
        public static void BeforeTestRun()
        {
            var appSettingsFactory = new AppSettingsFactory();
            _appSettings = appSettingsFactory.LoadAppSettings();
        }

        [BeforeFeature(Order = HookOrdering.BeforeAndAfterSetting)]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (featureContext == null) throw new ArgumentNullException(nameof(featureContext));

            featureContext.Set(_appSettings);
        }

        [BeforeScenario(Order = HookOrdering.BeforeAndAfterSetting)]
        public void BeforeScenario()
        {
            scenarioContext.Set(_appSettings);
        }

        [AfterTestRun(Order = HookOrdering.AfterTestRun)]
        public static void ResetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("EnvironmentUnderTest", null);
            Environment.SetEnvironmentVariable("LunchBrowserInHeadlessMode", null);
        }
    }
}
