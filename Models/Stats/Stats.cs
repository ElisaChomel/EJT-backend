using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace judo_backend.Models.Stats
{
    public class Stats
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [Required]
        [JsonPropertyName("pageName")]
        public string PageName { get; set; }

        [Required]
        [JsonPropertyName("countView")]
        public int CountView { get; set; }
    }
}
