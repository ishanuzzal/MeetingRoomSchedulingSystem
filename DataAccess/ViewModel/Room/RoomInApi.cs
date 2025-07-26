using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.Room
{
    public class RoomInApi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
    }
}
