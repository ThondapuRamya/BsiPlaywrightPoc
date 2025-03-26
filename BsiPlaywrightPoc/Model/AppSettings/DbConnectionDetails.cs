namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class DbConnectionDetails
    {
        public string? StandardDb { get; set; }
        public string? CustomerProfileDb { get; set; }
        public string? EntitlementDb { get; set; }
        public string? SapIntegrationDb { get; set; }
        public string? DwhDb { get; set; }
        public DbCredentials? DbCredentials { get; set; }
    }
}
