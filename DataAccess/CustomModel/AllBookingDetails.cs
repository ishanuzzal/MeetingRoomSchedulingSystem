using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModel
{
    public class AllBookingDetails
    {
        public Booking Booking { get; set; }
        public string RoomName { get; set; }
        public string UserName { get; set; }
    }
}
