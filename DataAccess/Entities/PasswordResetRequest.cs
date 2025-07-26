using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PasswordResetRequest
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public DateTime RequestedTime {  get; set; }
        public DateTime ApprovedTime { get; set; }
        [Required]
        public string Status { get; set; }
        [StringLength(20, MinimumLength = 1)]
        public string? DefaultPassword { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
