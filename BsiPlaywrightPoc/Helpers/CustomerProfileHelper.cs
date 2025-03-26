using BsiPlaywrightPoc.Api;
using BsiPlaywrightPoc.Model.Api;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.RequestObject;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Helpers
{
    public class CustomerProfileHelper(ScenarioContext scenarioContext, ApiClient apiClient)
    {
        private AppSettings? _appSettings;

        public async Task<string?> SetUpVat(VatRequestObject vatRequestObject)
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            return await apiClient.PostAsync<string>(ClientsNames.CustomerProfile,
                _appSettings!.CustomerProfileApi.CustomerProfileEndpoint.Vat, vatRequestObject);
        }

        public async Task<string?> SetUpCodiceFiscale(CodiceFiscaleRequestObject codiceFiscaleRequestObject)
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            return await apiClient.PostAsync<string>(ClientsNames.CustomerProfile,
                _appSettings!.CustomerProfileApi.CustomerProfileEndpoint.CodiceFiscale, codiceFiscaleRequestObject);
        }
    }
}
