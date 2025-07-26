 using DataAccess.CustomValidation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DataAccess.ViewModel.Room
{
    public class AddRoomView : IValidatableObject
    {
        [Required]
        [Range(1, 10, ErrorMessage = $"{nameof(Floor)} must be between 1 and 10")]
        public int Floor { get; set; }
        [Required]
        [StringLength(20,MinimumLength=1, ErrorMessage = $"{nameof(RoomName)} length have to be 1-12 character")]
        public string RoomName { get; set; } = string.Empty;
        [Required]
        [Range(1, 100, ErrorMessage = $"{nameof(Capacity)} length must be between 1 and 100")]
        public int Capacity { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = $"{nameof(MinPersonLimit)} length must be between 1 and 100")]
        public int MinPersonLimit { get; set; }
        [Required]
        public string ColorCode { get; set; }
        [Required(ErrorMessage = $"{nameof(Facilities)} must have values")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = $"{nameof(Facilities)} length must be between 1 and 150")]
        public string Facilities { get; set; }

        [Required(ErrorMessage = $"{nameof(Image)} is required")]
        public IFormFile Image {  get; set; }   
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPersonLimit > Capacity)
            {
                yield return new ValidationResult(
                    "The minimum person limit cannot exceed the capacity."
                );
            }
        }

    }
}
