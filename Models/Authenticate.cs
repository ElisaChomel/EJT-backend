using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class Authenticate
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
