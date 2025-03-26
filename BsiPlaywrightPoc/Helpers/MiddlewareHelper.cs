using BsiPlaywrightPoc.Api;
using BsiPlaywrightPoc.Model.Api;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.RequestObject;
using BsiPlaywrightPoc.Model.ResponseObjects.User;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Helpers
{
    public class MiddlewareHelper(ScenarioContext scenarioContext, ApiClient client)
    {
        private AppSettings? _appSettings;

        public async Task SignUp(UserSignUpRequestObject userSignUpRequestObject)
        {
            var csrfTokenAndHeaders = await GetCsrfTokenAndHeaders();
            var rawAndDeserializeResponseObject = await client.PostAsync<SignUpResponseObject>(ClientsNames.Middleware, _appSettings!.MiddlewareApi.MiddlewareApiEndpoint.SignUp, userSignUpRequestObject, csrfTokenAndHeaders.headers);
            rawAndDeserializeResponseObject.Item2?.User!.Email.Should().Be(userSignUpRequestObject.email);
        }

        public async Task LoginToExtractAuthTokenAsync(object loginDetails)
        {
            var csrfTokenAndHeaders = await GetCsrfTokenAndHeaders();
            _ = await client.PostAsync<string>(ClientsNames.Middleware, _appSettings!.MiddlewareApi.MiddlewareApiEndpoint.Login, loginDetails, csrfTokenAndHeaders.headers);
        }

        private async Task<(CsrfTokenResponseObject deserializedResponse, SpecialRequestHeaders headers)> GetCsrfTokenAndHeaders()
        {
            string? domainCookie = null;
            _appSettings = scenarioContext.Get<AppSettings>();

            var rawAndDeserializeResponseObject = await client.GetAsync<CsrfTokenResponseObject>(ClientsNames.Middleware, _appSettings.MiddlewareApi.MiddlewareApiEndpoint.CsrfToken);

            var cookieName = ExtractCookie(rawAndDeserializeResponseObject.httpResponseMessage, ref domainCookie);

            var headers = new SpecialRequestHeaders()
            {
                ApiHeaders = new List<KeyValuePair<string, string>>(new[]
                {
                    new KeyValuePair<string, string>("x-csrf-token", rawAndDeserializeResponseObject.Item2!.CsrfToken!),
                    new KeyValuePair<string, string>("Cookie", $"{cookieName}={domainCookie}")
                })
            };

            return (rawAndDeserializeResponseObject.Item2, headers);
        }

        private string ExtractCookie(HttpResponseMessage rawResponse, ref string? domainCookie)
        {
            var cookieName = _appSettings!.RuntimeSettings.Environment.ToLower() switch
            {
                "uat" => "accord-uat",
                "ppd" => "accord-ppd",
                _ => "accord-main"
            };

            if (!rawResponse.Headers.TryGetValues("Set-Cookie", out var cookies)) return cookieName;
            foreach (var cookie in cookies)
            {
                var cookieParts = cookie.Split(';');
                foreach (var part in cookieParts)
                {
                    var keyValue = part.Split('=');
                    if (keyValue[0].Trim() == cookieName)
                    {
                        domainCookie = keyValue[1].Trim();
                    }
                }
            }

            return cookieName;
        }
    }
}
