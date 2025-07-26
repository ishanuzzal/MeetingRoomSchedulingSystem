using DataAccess.CustomValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Room
{
    public class UpdateRoomView : IValidatableObject
    {
        [Required]
        public int Id { get; set; } 
        [Required]
        [Range(1, 10, ErrorMessage = $"{nameof(Floor)} must be between 1 and 10")]
        public int Floor { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Room have to be 1-12 character")]
        public string RoomName { get; set; } = string.Empty;
        [Required]
        public string ColorCode { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = $"{nameof(Capacity)} must be between 1 and 100")]
        public int Capacity { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = $"{nameof(MinPersonLimit)} must be between 1 and 100")]
        public int MinPersonLimit { get; set; }
        [Required(ErrorMessage = $"{nameof(Facilities)} must be between 1 and 100")]
        public string Facilities { get; set; }
        public IFormFile? Photo {  get; set; }  
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
