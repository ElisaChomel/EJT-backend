using judo_backend.Models.Enum;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class EjtPerson
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public EjtPersonType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("photoname")]
        public string PhotoName { get; set; }
    }
}
