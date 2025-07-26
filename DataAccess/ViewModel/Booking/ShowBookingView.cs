using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Booking
{
    public class ShowBookingView
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
        public string UserEmail { get; set; }

        public string ColorCode { get; set; }

        [Required]
        public int RoomId { get; set; }

        public bool IsOwner { get; set; } = false;
    }

}
