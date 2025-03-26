using BsiPlaywrightPoc.Model.ResponseObjects;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;

namespace BsiPlaywrightPoc.Helpers
{
    public class FeatureFlagHelper
    {
        public async Task<FeatureFlagResponseObject?> GetFeatureFlagsAsync(Uri requestUri)
        {
            using var client = new HttpClient();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(true);
                var res = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                return JsonConvert.DeserializeObject<FeatureFlagResponseObject>(res);
            }
            catch (Exception exception) when (exception is WebException or HttpRequestException or SocketException)
            {
                //Logger.Write($"Get request failed on this endpoint : {endpoint}, and Inner exception was: ", exception.InnerException, EventSeverity.Error);
                throw;
            }
        }
    }
}
