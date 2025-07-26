using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomException
{
    public class RoomPersonRangeException:Exception
    {
        public RoomPersonRangeException(string message):base(message) {
        
        }
    }
}
