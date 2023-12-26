using judo_backend.Models.Enum;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
