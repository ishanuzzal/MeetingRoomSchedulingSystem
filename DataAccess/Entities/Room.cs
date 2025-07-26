using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Room
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string QrCodeIdentifier { get; set; }
        [Required]
        public int Floor { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Name { get; set; } 
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int MinPersonLimit { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Facilities { get; set; }
        [Required]
        [StringLength(7, MinimumLength = 1)]
        public string ColorCode { get; set; }
        [Required]
        public string QRCode { get; set; }
        [Required]
        public string ImageName { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
