using System.Text.Json.Serialization;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Dtos
{
    public class CreateCheckInDto
    {
        public int Mood { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductivityLevel Productivity { get; set; }

        public string? Notes { get; set; }
    }
}