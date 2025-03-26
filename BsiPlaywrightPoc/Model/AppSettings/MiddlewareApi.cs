namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class MiddlewareApi
    {
        public required string HostName { get; set; }
        public required MiddlewareApiEndpoint MiddlewareApiEndpoint { get; set; }
    }
}
