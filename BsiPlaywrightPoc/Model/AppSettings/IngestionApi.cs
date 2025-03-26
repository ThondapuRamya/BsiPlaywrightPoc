namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class IngestionApi
    {
        public required string HostName { get; set; }
        public required IngestionEndpoint IngestionEndpoint { get; set; }
    }
}
