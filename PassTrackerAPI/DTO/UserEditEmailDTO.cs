using System.ComponentModel.DataAnnotations;

namespace PassTrackerAPI.DTO
{
    public class UserEditEmailDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
