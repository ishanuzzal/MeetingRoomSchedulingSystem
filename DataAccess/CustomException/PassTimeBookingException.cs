using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomException
{
    public class PassTimeBookingException:Exception
    {
        public PassTimeBookingException(string message):base(message) { }
    }
}
