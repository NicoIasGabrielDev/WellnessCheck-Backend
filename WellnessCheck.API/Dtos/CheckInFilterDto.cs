using System;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Dtos
{
    public class CheckInFilterDto
    {
        public Guid? UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Mood { get; set; }
        public ProductivityLevel? Productivity { get; set; }
    }
}
