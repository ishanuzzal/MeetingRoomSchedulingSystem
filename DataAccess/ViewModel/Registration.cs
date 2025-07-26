using DataAccess.CustomValidation;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.ViewModel
{
    public class Registration
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [IdentityPasswordValidation]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Range(1, 100000, ErrorMessage = "Employee Id must be between 1 and 100000")]
        [Required]
        public int EmpId { get; set; }

        [RegularExpression("[A-Za-z\\s]*", ErrorMessage = "The name can only have english characters")]
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Last name should be between 4 and 30 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string SelectedRole { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }

        [Required(ErrorMessage = "Please select a department.")]
        public int? SelectedDepartment { get; set; }
        public IEnumerable<SelectListItem>? Departments { get; set; } 

        [Required(ErrorMessage = "Please select a designation.")]
        public int? SelectedDesignation { get; set; }
        public IEnumerable<SelectListItem>? Designations { get; set; }
    }

}
