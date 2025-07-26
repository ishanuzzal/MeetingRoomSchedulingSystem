using System.ComponentModel.DataAnnotations;

namespace DataAccess.ViewModel.User
{
    public class AUserBookingView
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Purpose { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int Floor {  get; set; } 

        [Required]
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        [Required]
        public int NumberOfParticipation { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public string ColorCode { get; set; }

        [Required]
        public string RoomName { get; set; }

    }
}
