using Microsoft.Extensions.Configuration;

namespace BsiPlaywrightPoc.Factory.AppSettings
{
    public class AppSettingsFactory
    {
        private readonly IConfiguration _configuration;
        private Model.AppSettings.AppSettings? _appSettings;

        public AppSettingsFactory()
        {
            var env = Environment.GetEnvironmentVariable("EnvironmentUnderTest") ?? "local";
            Environment.SetEnvironmentVariable("EnvironmentUnderTest", env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public IConfiguration GetLoggingConfiguration()
        {
            return _configuration.GetSection("Logging");
        }

        public Model.AppSettings.AppSettings LoadAppSettings()
        {    
            // Initialize with default! to avoid compiler warning
            _appSettings = new Model.AppSettings.AppSettings
            {
                DbConnectionDetails = default!,
                KnowledgeBasicAuth = default!,
                ApiBasicAuth = default!,
                BsiKnowledge = default!,
                EntitlementApi = default!,
                MiddlewareApi = default!,
                IngestionApi = default!,
                RuntimeSettings = default!,
                CustomerProfileApi = default!
            };

            _configuration.Bind(_appSettings);
            return _appSettings;
        }
    }
}
