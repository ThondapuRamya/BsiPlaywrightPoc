using System.Net.Http.Headers;
using System.Net.Sockets;
using BsiPlaywrightPoc.Model.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BsiPlaywrightPoc.Api
{
    public class ApiClient(IHttpClientFactory httpClientFactory)
    {
        private const string JsonDefaultContentType = "application/json";

        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public async Task<(HttpResponseMessage httpResponseMessage, T?)> GetAsync<T>(string clientsName, string endpoint) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                var deserializeResponseObject = JsonConvert.DeserializeObject<T>(res);

                return (response, deserializeResponseObject);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string clientsName, string endpoint, BasicAuth basicAuth) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            var byteArray = new System.Text.UTF8Encoding().GetBytes($"{basicAuth.Username}:{basicAuth.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                return JsonConvert.DeserializeObject<T>(res);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }

        public async Task<(HttpResponseMessage httpResponseMessage, T?)> PostAsync<T>(string clientsName, string endpoint, object requestPayload, SpecialRequestHeaders specialRequestHeaders) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            foreach (var (key, value) in specialRequestHeaders.ApiHeaders!)
            {
                client.DefaultRequestHeaders.Add(key, value);
            }

            var content = requestPayload is string
                ? requestPayload.ToString()
                : await Task.Factory.StartNew(() => JsonConvert.SerializeObject(requestPayload, Formatting.Indented, JsonSerializerSettings));

            var requestContent = new StringContent(content!, System.Text.Encoding.UTF8, JsonDefaultContentType);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Content = requestContent;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                var deserializeResponseObject = JsonConvert.DeserializeObject<T>(res);

                return (response, deserializeResponseObject);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string clientsName, string endpoint, object requestPayload, BasicAuth? basicAuth = null) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            if (basicAuth != null)
            {
                var byteArray = new System.Text.UTF8Encoding().GetBytes($"{basicAuth.Username}:{basicAuth.Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            var content = requestPayload is string
                ? requestPayload.ToString()
                : await Task.Factory.StartNew(() => JsonConvert.SerializeObject(requestPayload, Formatting.Indented, JsonSerializerSettings));

            var requestContent = new StringContent(content!, System.Text.Encoding.UTF8, JsonDefaultContentType);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Content = requestContent;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                return JsonConvert.DeserializeObject<T>(res);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string clientsName, string endpoint, List<string> filePathToUpload, BasicAuth? basicAuth = null) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            if (basicAuth != null)
            {
                var byteArray = new System.Text.UTF8Encoding().GetBytes($"{basicAuth.Username}:{basicAuth.Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            using var formData = new MultipartFormDataContent();
            foreach (var filePath in filePathToUpload)
            {
                var fileContent = new StreamContent(File.OpenRead(filePath));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formData.Add(fileContent, "files", Path.GetFileName(filePath));
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Content = formData;

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                return JsonConvert.DeserializeObject<T>(res);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }

        public async Task<T?> DeleteAsync<T>(string clientsName, string endpoint, object requestPayload, BasicAuth? basicAuth = null) where T : class
        {
            var client = httpClientFactory.CreateClient(clientsName);

            if (basicAuth != null)
            {
                var byteArray = new System.Text.UTF8Encoding().GetBytes($"{basicAuth.Username}:{basicAuth.Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            var content = requestPayload is string
                ? requestPayload.ToString()
                : await Task.Factory.StartNew(() => JsonConvert.SerializeObject(requestPayload, Formatting.Indented, JsonSerializerSettings));

            var requestContent = new StringContent(content!, System.Text.Encoding.UTF8, JsonDefaultContentType);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                request.Content = requestContent;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var res = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) throw new Exception($"Failed on: {response.RequestMessage?.RequestUri?.AbsoluteUri} Endpoint, and Response body returned was: {res}");

                return JsonConvert.DeserializeObject<T>(res);
            }
            catch (Exception exception) when (exception is HttpRequestException or SocketException)
            {
                throw;
            }
        }
    }
}
