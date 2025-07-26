using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModel
{
    public class PaginatedBookingModel
    {
        public List<Booking> bookings { get; set; } = [];
        public int TotalBooking { get; set; }
    }
}
