using Newtonsoft.Json;

namespace BsiPlaywrightPoc.Model.ResponseObjects
{
    public class DeleteStandardResponseObject
    {
        [JsonProperty(PropertyName = "deletedSapIds")]
        public List<string>? DeletedSapIds { get; set; }

        [JsonProperty(PropertyName = "notFoundSapIds")]
        public List<string>? NotFoundSapIds { get; set; }
    }
}
