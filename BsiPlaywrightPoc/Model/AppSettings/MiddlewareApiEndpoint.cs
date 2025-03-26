namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class MiddlewareApiEndpoint
    {
        public required string FeatureFlag { get; set; }
        public required string CsrfToken { get; set; }
        public required string SignUp { get; set; }
        public required string Login { get; set; }
    }
}
