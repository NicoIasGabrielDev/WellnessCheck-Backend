using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Entities
{
    public class CheckIn
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Mood deve estar entre 1 e 5")]
        public int Mood { get; set; }

        [Required]
        [EnumDataType(typeof(ProductivityLevel))]
        public ProductivityLevel Productivity { get; set; }

        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
