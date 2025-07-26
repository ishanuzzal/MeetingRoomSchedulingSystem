using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class SystemSettingView
    {
        [Required(ErrorMessage ="This field is required")]
        public string MinimumMeetingTime { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string MaximumMeetingTime { get; set; }
    }
}
