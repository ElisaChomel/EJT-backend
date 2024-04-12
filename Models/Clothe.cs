using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class Clothe
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("ref")]
        public string Reference { get; set; }

        [JsonPropertyName("targetPoeple")]
        public string TargetPoeple { get; set; }

        [JsonPropertyName("jacketDescription")]
        public string JacketDescription { get; set; }

        [JsonPropertyName("pantDescription")]
        public string PantDescription { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("composition")]
        public string Composition { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }
    }
}
