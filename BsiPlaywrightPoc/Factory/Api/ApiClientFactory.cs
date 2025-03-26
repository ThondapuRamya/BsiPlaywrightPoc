using Microsoft.Extensions.DependencyInjection;
using System.Net;
using BsiPlaywrightPoc.Model.Api;

namespace BsiPlaywrightPoc.Factory.Api
{
    public class ApiClientFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CookieContainer _cookieContainer;

        public ApiClientFactory()
        {
            var serviceCollection = new ServiceCollection();
            _cookieContainer = new CookieContainer();

            serviceCollection.AddHttpClient();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IHttpClientFactory SetUpHttpClientFactory(IList<NamedApiClientObject> namedApiClientDetails)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_cookieContainer);
            foreach (var client in namedApiClientDetails)
            {
                serviceCollection.AddHttpClient(client.ClientName!, c =>
                    {
                        c.BaseAddress = new Uri(client.ClientBaseUrl!);
                        foreach (var (key, value) in client.ApiHeaders!)
                        {
                            c.DefaultRequestHeaders.Add(key, value);
                        }
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        AllowAutoRedirect = client.AllowAutoRedirect,
                        UseCookies = true,
                        CookieContainer = _cookieContainer,
                        ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                    });
            }

            var builtServiceProvider = serviceCollection.BuildServiceProvider();
            return builtServiceProvider.GetService<IHttpClientFactory>()!;
        }
    }
}