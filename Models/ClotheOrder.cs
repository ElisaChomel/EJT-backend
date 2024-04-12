using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class ClotheOrder
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("ref")]
        public string Reference { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("isPay")]
        public bool IsPay { get; set; }

        [JsonPropertyName("items")]
        public List<ClotheOrderItem> Items { get; set; }
    }
}
