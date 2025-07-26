using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.User
{
    public class EditUserViewModel
    {

        public string Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Range(1, 100000, ErrorMessage = "Employee Id must be between 1 and 100000")]
        [Required]
        public int EmpId { get; set; }

        [RegularExpression("[A-Za-z\\s]*", ErrorMessage = "The name can only have english characters")]
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Last name should be between 4 and 30 characters")]
        public string FullName { get; set; }
        [Required]
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
