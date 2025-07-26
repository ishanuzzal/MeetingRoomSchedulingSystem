using Microsoft.AspNetCore.Mvc.Rendering;

namespace Meeting_Scheduling_System.Models
{
    public class DropdownModel
    {
        public string SelectedRole { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
        public int? SelectedDepartment { get; set; }
        public IEnumerable<SelectListItem>? Departments { get; set; }
        public int? SelectedDesignation { get; set; }
        public IEnumerable<SelectListItem>? Designations { get; set; }
    }
}
