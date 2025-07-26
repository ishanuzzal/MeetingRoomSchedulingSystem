using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Room
{
    public class ShowRoomView
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string ColorCode { get; set; }
        public int Capacity { get; set; }
        public int MinPersonLimit { get; set; }
        public string Facilities { get; set; }
        public string? QRCode { get; set; }
        public string? ImageName { get; set; }
    }
}
