using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomException
{
    public class NoEmptySlotException:Exception
    {
        public NoEmptySlotException(string message):base(message)
        {
        
        }
    }
}
