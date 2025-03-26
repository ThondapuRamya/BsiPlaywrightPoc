using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.User
{
    public class CsrfTokenResponseObject
    {
        [JsonPropertyName("csrfToken")]
        public string? CsrfToken { get; set; }
    }
}
