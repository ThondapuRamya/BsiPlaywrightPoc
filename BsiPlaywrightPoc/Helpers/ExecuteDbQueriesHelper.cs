using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.AppSettings;
using System.Data;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BsiPlaywrightPoc.Helpers
{
    public static class ExecuteDbQueriesHelper
    {
        public static DataTable QueryStandardDb(this string? sapId, AppSettings appSettings, ILogger logger) =>
            appSettings.StandardDbConnectionString().SqlExecute(SelectQueryForStandards(sapId), logger)!;

        public static DataTable QueryOrderStateFromSapIntegrationDb(this string orderNumber, AppSettings appSettings, ILogger logger) =>
            appSettings.SapIntegrationDbConnectionString().SqlExecute(SelectQueryForOrderStateFromSapIntegrationDb(orderNumber), logger)!;

        public static DataTable QueryOrdersFromDwhDb(this string orderNumber, AppSettings appSettings, ILogger logger) =>
            appSettings.DwhDbConnectionString().SqlExecute(SelectQueryForOrdersFromDwhDb(orderNumber), logger)!;

        public static DataTable QueryCustomerProfile(this string email, AppSettings appSettings, ILogger logger) => appSettings.CustomerProfileDbConnectionString().SqlExecute($"SELECT* FROM[dbo].[CustomerProfiles] WHERE Email = '{email}'", logger)!;
        
        private static string SelectQueryForOrdersFromDwhDb(this string orderNumber) => $"SELECT [Number], [PaymentType] FROM [dbo].[Orders] WHERE Number = '{orderNumber}';";

        private static string SelectQueryForOrderStateFromSapIntegrationDb(this string orderNumber) => 
            $"SELECT * FROM [dbo].[OrderState] WHERE ShopifyOrderNumber = '{orderNumber}';";

        private static string SelectQueryForStandards(this string? sapId) => 
            $"SELECT* FROM[dbo].[Standards] WHERE SapId = '{sapId}'";

        private static string StandardDbConnectionString(this AppSettings appSettings) => 
            $"Server={appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={appSettings.DbConnectionDetails.StandardDb};User ID={appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";

        private static string SapIntegrationDbConnectionString(this AppSettings appSettings) => 
            $"Server={appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={appSettings.DbConnectionDetails.SapIntegrationDb};User ID={appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";

        private static string DwhDbConnectionString(this AppSettings appSettings) => 
            $"Server={appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={appSettings.DbConnectionDetails.DwhDb};User ID={appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
        private static string CustomerProfileDbConnectionString(this AppSettings appSettings) =>
            $"Server={appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={appSettings.DbConnectionDetails.CustomerProfileDb};User ID={appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
}
}
