using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomException
{
    public class HavePendingRequestException:Exception
    {
        public HavePendingRequestException(string message):base(message) { }    
    }
}
