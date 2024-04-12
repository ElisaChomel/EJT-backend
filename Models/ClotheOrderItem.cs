using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class ClotheOrderItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("ref")]
        public string Reference { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }
    }
}
