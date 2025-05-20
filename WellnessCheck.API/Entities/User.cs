using System.ComponentModel.DataAnnotations;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; } = Role.Employee;

        public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    }
}
