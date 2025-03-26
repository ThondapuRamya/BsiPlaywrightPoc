using BoDi;
using TechTalk.SpecFlow;
using System.Net;
using BsiPlaywrightPoc.Factory.Api;
using BsiPlaywrightPoc.Model.Api;
using BsiPlaywrightPoc.Model.AppSettings;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public class ApiHooks(FeatureContext featureContext, IObjectContainer container)
    {
        private FeatureContext _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        private readonly IObjectContainer _container = container;
        private static AppSettings? _appSettings;

        [BeforeFeature(Order = HookOrdering.BetweenFeatureAndScenarioSetting)]
        public static void BeforeTestRun(FeatureContext featureContext, IObjectContainer container)
        {
            if (featureContext == null) throw new ArgumentNullException(nameof(featureContext));
            _appSettings = featureContext.Get<AppSettings>();

            var apiClientFactory = new ApiClientFactory();
            var httpClientFactory = apiClientFactory.SetUpHttpClientFactory(ApiClients());
            var cookieContainer = new CookieContainer();

            if (httpClientFactory == null)
            {
                throw new InvalidOperationException("Failed to initialize IHttpClientFactory");
            }

            container.RegisterInstanceAs(httpClientFactory);
            container.RegisterInstanceAs(cookieContainer);
        }

        private static IList<NamedApiClientObject> ApiClients()
        {
            return new List<NamedApiClientObject>
            {
                new NamedApiClientObject
                {
                    ClientName = ClientsNames.Middleware,
                    ClientBaseUrl = _appSettings!.MiddlewareApi.HostName,
                    ApiHeaders = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Accept", "application/json")
                    }
                },
                new NamedApiClientObject
                {
                    ClientName = ClientsNames.Ingestion,
                    ClientBaseUrl = _appSettings.IngestionApi.HostName,
                    ApiHeaders = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Accept", "application/json")
                    }
                },
                new NamedApiClientObject
                {
                    ClientName = ClientsNames.CustomerProfile,
                    ClientBaseUrl = _appSettings.CustomerProfileApi.HostName,
                    ApiHeaders = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Accept", "application/json")
                    }
                }
            };
        }
    }
}