namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class RuntimeSettings
    {
        public required string Environment { get; set; }
        public required string BrowserType { get; set; }
        public bool LunchBrowserInHeadlessMode { get; set; }
        public string? MailsacApiKey { get; set; }
    }
}
