using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Booking
{
    public class UpdateBookingView
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Purpose { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        [Required]
        public int NumberOfParticipation { get; set; }
        [Required]
        public string AppUserId { get; set; }
        public string ColorCode { get; set; }
        [Required]
        public int RoomId { get; set; }
    }
}
