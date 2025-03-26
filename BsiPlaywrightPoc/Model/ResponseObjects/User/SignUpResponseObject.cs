using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.User
{
    public class SignUpResponseObject
    {
        [JsonPropertyName("multiPassToken")]
        public string? MultiPassToken { get; set; }

        [JsonPropertyName("user")]
        public UserResponseObject? User { get; set; }
    }
}
