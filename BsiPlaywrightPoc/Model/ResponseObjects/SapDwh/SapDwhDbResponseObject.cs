using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.SapDwh
{
    public class SapDwhDbResponseObject
    {
        [JsonPropertyName("sapItemsData")]
        public required List<SapItemsDatum> SapItemsData { get; set; }

        [JsonPropertyName("orderStatus")]
        public required string OrderStatus { get; set; }
    }
}
