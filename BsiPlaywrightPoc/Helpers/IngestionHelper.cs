using BsiPlaywrightPoc.Api;
using BsiPlaywrightPoc.Model;
using BsiPlaywrightPoc.Model.Api;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.RequestObject;
using BsiPlaywrightPoc.Model.ResponseObjects;
using TechTalk.SpecFlow;
using BasicAuth = BsiPlaywrightPoc.Model.Api.BasicAuth;

namespace BsiPlaywrightPoc.Helpers
{
    public class IngestionHelper(ScenarioContext scenarioContext, ApiClient client)
    {
        private AppSettings? _appSettings;

        public async Task IngestStandard(Standard standard)
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            var basicAuth = new BasicAuth
            {
                Username = _appSettings.ApiBasicAuth.UserName,
                Password = _appSettings.ApiBasicAuth.Password
            };

            foreach (var file in standard.Files)
            {
                await client.PostAsync<string>(ClientsNames.Ingestion, _appSettings.IngestionApi.IngestionEndpoint.Upload, [file], basicAuth);
            }
        }

        public async Task<DeleteStandardResponseObject?> DeleteIngestedStandard(Standard standard)
        {
            _appSettings = scenarioContext.Get<AppSettings>();

            var basicAuth = new BasicAuth
            {
                Username = _appSettings.ApiBasicAuth.UserName,
                Password = _appSettings.ApiBasicAuth.Password
            };

            var deletionPayload = new DeleteIngestedStandardObject
            {
                SapIds = [standard.SapId!],
                DeletionReason = $"Preparing ingestion of {standard.SapId} for E2E"
            };
            
            return await client.DeleteAsync<DeleteStandardResponseObject>(ClientsNames.Ingestion, _appSettings.IngestionApi.IngestionEndpoint.Delete, deletionPayload, basicAuth);
        }
    }
}
