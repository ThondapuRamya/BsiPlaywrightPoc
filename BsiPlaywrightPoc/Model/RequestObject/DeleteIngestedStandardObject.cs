namespace BsiPlaywrightPoc.Model.RequestObject
{
    public class DeleteIngestedStandardObject
    {
        public List<string>? SapIds { get; set; }
        public string? DeletionReason { get; set; }
    }
}
