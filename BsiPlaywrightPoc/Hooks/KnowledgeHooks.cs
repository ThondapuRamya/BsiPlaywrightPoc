using BsiPlaywrightPoc.Api;
using BsiPlaywrightPoc.Model.Api;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.ResponseObjects;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class KnowledgeHooks(
        FeatureContext featureContext,
        ApiClient client,
        IUnitTestRuntimeProvider unitTestRuntimeProvider)
    {
        private readonly FeatureContext _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        private readonly ApiClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider = unitTestRuntimeProvider ?? throw new ArgumentNullException(nameof(unitTestRuntimeProvider));

        [BeforeFeature(Order = HookOrdering.ScenarioHooksSetting)]
        public static async Task BeforeFeature(FeatureContext featureContext, IUnitTestRuntimeProvider unitTestRuntimeProvider, ApiClient client)
        {
            if (featureContext == null) throw new ArgumentNullException(nameof(featureContext));
            if (unitTestRuntimeProvider == null) throw new ArgumentNullException(nameof(unitTestRuntimeProvider));
            if (client == null) throw new ArgumentNullException(nameof(client));

            var appSettings = featureContext.Get<AppSettings>();

            if (appSettings.MiddlewareApi.MiddlewareApiEndpoint.FeatureFlag == null)
            {
                throw new ArgumentNullException(nameof(appSettings.MiddlewareApi.MiddlewareApiEndpoint.FeatureFlag));
            }

            var featureUnderTest = featureContext.FeatureInfo.Title;

            var featureFlags = await client.GetAsync<FeatureFlagResponseObject>(ClientsNames.Middleware,
                appSettings.MiddlewareApi.MiddlewareApiEndpoint.FeatureFlag);

            if (!featureFlags.Item2!.MarketingPreferences && featureUnderTest == "MarketingPreference")
            {
                unitTestRuntimeProvider.TestInconclusive(
                    $"{featureUnderTest} Feature flag is {featureFlags.Item2.MarketingPreferences}, therefore every scenario under {featureUnderTest} feature will be skipped {Environment.NewLine}");
            }
        }
    }
}
