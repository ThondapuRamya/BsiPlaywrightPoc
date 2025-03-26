using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Helpers
{
    public class RequestLoggerHelper(string urlPattern)
    {
        private readonly List<IRequest> _requests = [];
        private readonly List<IResponse> _responses = [];

        public void StartLogging(IPage page)
        {
            page.Request += (_, request) =>
            {
                if (request.Url.Contains(urlPattern))
                {
                    _requests.Add(request);
                }
            };

            page.Response += (_, response) =>
            {
                if (response.Request.Url.Contains(urlPattern))
                {
                    _responses.Add(response);
                }
            };
        }

        public List<IRequest> Requests => _requests;
        public List<IResponse> Responses => _responses;
    }
}
