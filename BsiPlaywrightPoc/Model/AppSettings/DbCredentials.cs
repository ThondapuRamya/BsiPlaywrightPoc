namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class DbCredentials
    {
        public required string ServerName { get; set; }
        public required string DatabaseUser { get; set; }
        public required string DatabasePassword { get; set; }
    }
}
