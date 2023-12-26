using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class Agenda
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [Required]
        [JsonPropertyName("resume")]
        public string Resume { get; set; }

        [Required]
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
