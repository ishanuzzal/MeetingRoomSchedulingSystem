using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class MeetingTimeLimit
    {
        [Key]
        public int Id { get; set; }
        public int MinimumMinuteTime { get; set; }
        public int MaximumMinuteTime { get; set; }
    }
}
