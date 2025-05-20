using System.Text.Json.Serialization;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Dtos
{
    public class CheckInResponseDto
    {
        public Guid Id { get; set; }
        public int Mood { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductivityLevel Productivity { get; set; }

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
