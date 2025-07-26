using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Booking
{
    public class AddBookingAdminView
    {
        [Required]
        public string Purpose { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Pending";
        [Required]
        public int NumberOfParticipation { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string AppUserId { get; set; }
        [Required]
        public int RoomId { get; set; }
    }


}
