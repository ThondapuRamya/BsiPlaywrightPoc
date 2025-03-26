namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class CustomerProfileApi
    {
        public required string HostName { get; set; }
        public required CustomerProfileEndpoint CustomerProfileEndpoint { get; set; }
    }
}
