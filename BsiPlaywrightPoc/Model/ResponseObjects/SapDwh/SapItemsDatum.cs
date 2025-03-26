using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.SapDwh
{
    public class SapItemsDatum
    {
        [JsonPropertyName("productId")]
        public string? ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("sapOrderNumber")]
        public string? SapOrderNumber { get; set; }
    }
}
