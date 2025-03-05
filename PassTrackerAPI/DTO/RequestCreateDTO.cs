using PassTrackerAPI.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PassTrackerAPI.DTO
{
    public class RequestCreateDTO : IValidatableObject
    {
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public TypeRequestDB TypeRequest { get; set; }
        
        [AllowNull]
        public byte[]? Photo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > FinishDate)
            {
                yield return new ValidationResult(
                    $"StartDate can't be later than FinishDate",
                    new[] { nameof(StartDate) });
            }
        }
    }
}
