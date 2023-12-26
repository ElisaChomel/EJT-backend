using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class Stage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("start")]
        public DateTime Start { get; set; }

        [Required]
        [JsonPropertyName("end")]
        public DateTime End { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [Required]
        [JsonPropertyName("yearBirthdayMin")]
        public int YearBirthdayMin { get; set; }

        [Required]
        [JsonPropertyName("yearBirthdayMax")]
        public int YearBirthdayMax { get; set; }

        [Required]
        [JsonPropertyName("maxInscriptionDate")]
        public DateTime MaxInscriptionDate { get; set; }
    }
}
