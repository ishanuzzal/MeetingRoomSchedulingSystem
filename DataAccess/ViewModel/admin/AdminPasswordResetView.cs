using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.CustomValidation;

namespace DataAccess.ViewModel.admin
{
    public class AdminPasswordResetView
    {
        public int Id { get; set; } 
        public DateTime RequestedTime { get; set; }
        public DateTime ApprovedTime { get; set; }
        public string? Status { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [IdentityPasswordValidation]
        public string? DefaultPassword { get; set; }
        public string? Email { get; set; }
    }
}
