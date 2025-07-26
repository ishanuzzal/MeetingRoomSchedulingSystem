using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class CreateRole
    {
        [Required]
        [Display(Name = "Role")]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters long.")]
        public string RoleName { get; set; } = null!;
    }
}
