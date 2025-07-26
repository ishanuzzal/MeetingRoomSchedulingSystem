using DataAccess.TimeAuditable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Booking: ITimeAuditable
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Purpose { get; set; } 
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Status { get; set; } 
        [Required]
        public int NumberOfParticipation { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public AppUser AppUser { get; set; } 
        public Room Room { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt{ get; set; }
    }
}
