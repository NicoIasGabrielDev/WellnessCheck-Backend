namespace WellnessCheck.API.Dtos
{
    public class CheckInStatsDto
    {
        public string Period { get; set; } // Ex: "2024-W20" ou "2024-05"
        public double AvgMood { get; set; }
        public double AvgProductivity { get; set; }
    }
}
