using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class UpdateMeetingTimeView
    {
        [Required]
        public IEnumerable<string> MinimumMeetingTime { get; set; }
        [Required]
        public IEnumerable<string> MaximumMeetingTime { get; set; }
        
        public string SelectedMinTime { get; set; }
        public string SelectedMaxTime { get; set; }
    }
}
