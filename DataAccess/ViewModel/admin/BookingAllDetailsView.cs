using DataAccess.TimeAuditable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class BookingAllDetailsView
    {
        [Required]
        public int Id { get; set; } 

        [Required]
        public string Purpose { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int NumberOfParticipation { get; set; }
        public string? UserName { get; set; }
        public int? EmpId { get; set; }
        public int? Floor { get; set; }
        public string? RoomName { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? RequestedAtUtc { get; set; }
       
    }
}
