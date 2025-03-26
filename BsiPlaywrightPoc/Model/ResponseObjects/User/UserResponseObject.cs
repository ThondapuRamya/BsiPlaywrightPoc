using System.Text.Json.Serialization;

namespace BsiPlaywrightPoc.Model.ResponseObjects.User
{
    public class UserResponseObject
    {
        [JsonPropertyName("organisation")]
        public object? Organisation { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("referenceId")]
        public string? ReferenceId { get; set; }

        [JsonPropertyName("referenceIdSource")]
        public string? ReferenceIdSource { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public object? UpdatedAt { get; set; }

        [JsonPropertyName("roles")]
        public List<object>? Roles { get; set; }

        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}
