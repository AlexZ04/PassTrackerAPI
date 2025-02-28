using PassTrackerAPI.Data;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PassTrackerAPI.DTO
{
    public class UserRegisterDTO
    {
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
        public int? Group { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

}
