namespace BsiPlaywrightPoc.Model
{
    public class Standard
    {
        public required string Name { get; set; }
        public required string Href { get; set; }
        public required string Title { get; set; }
        public required string? SapId { get; set; }
        public string? Id { get; set; }
        public required List<string> Files { get; set; }
        public List<string>? RefIds { get; set; }
        public List<string>? PrevVersions { get; set; }
        public string? SeriesId { get; set; }
        public string? TrackedChangesSapId { get; set; }
        public string? ShopifyProductId { get; set; }
    }
}
