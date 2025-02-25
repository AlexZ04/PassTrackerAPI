using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PassTrackerAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string SecondName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [AllowNull]
        public string GroupNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
