using System.Text.Json.Serialization;

namespace judo_backend.Models
{
    public class CompetitionResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("competition_id")]
        public int Competition_ID { get; set; }

        [JsonPropertyName("adherent_id")]
        public int Adherent_ID { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("position")]
        public int? Position { get; set; }
    }
}
