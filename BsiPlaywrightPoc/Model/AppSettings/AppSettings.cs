namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class AppSettings
    {
        public required DbConnectionDetails DbConnectionDetails { get; set; }
        public required KnowledgeBasicAuth KnowledgeBasicAuth { get; set; }
        public required ApiBasicAuth ApiBasicAuth { get; set; }
        public required BsiKnowledge BsiKnowledge { get; set; }
        public required MiddlewareApi MiddlewareApi { get; set; }
        public required IngestionApi IngestionApi { get; set; }
        public required CustomerProfileApi CustomerProfileApi { get; set; }
        public required EntitlementApi EntitlementApi { get; set; }
        public User? User { get; set; }
        public required RuntimeSettings RuntimeSettings { get; set; }
    }
}
