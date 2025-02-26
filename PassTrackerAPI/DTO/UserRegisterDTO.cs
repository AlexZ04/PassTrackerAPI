using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PassTrackerAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string SecondName { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string MiddleName { get; set; }
        [AllowNull]
        public string? GroupNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
