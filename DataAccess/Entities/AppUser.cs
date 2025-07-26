using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        public int EmpId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string FullName { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }  
        public int? DesignationId { get; set; }
        [ForeignKey("DesignationId")]
        public Designation? Designation { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<PasswordResetRequest> PasswordResetRequests { get; set; }  
    }
}
