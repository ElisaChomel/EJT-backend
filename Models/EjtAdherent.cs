using judo_backend.Models.Enum;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class EjtAdherent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("birthday")]
        public DateTime Birthday { get; set; }

        [JsonPropertyName("licenceCode")]
        public string LicenceCode { get; set; }

        [JsonPropertyName("weight")]
        public float? Weight { get; set; }

        [JsonPropertyName("belt")]
        public BeltType? Belt { get; set; }
    }
}
