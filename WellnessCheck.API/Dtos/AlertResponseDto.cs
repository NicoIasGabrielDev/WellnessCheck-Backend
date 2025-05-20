namespace WellnessCheck.API.Dtos
{
    public class AlertResponseDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ConsecutiveBadDays { get; set; }
    }
}
