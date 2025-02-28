using PassTrackerAPI.Data;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PassTrackerAPI.DTO
{
    public class UserRegisterDTO //: IValidatableObject
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
        

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
            

        //    if(Email == "user@example.com")
        //    {
        //        yield return new ValidationResult(
        //            $"Email error",
        //            new[] { nameof(Email) });
        //    }
        //    if (FirstName == "string")
        //    {
        //        yield return new ValidationResult(
        //            $"FirstName error.",
        //            new[] { nameof(Email) });
        //    }
        //}
    }

}
