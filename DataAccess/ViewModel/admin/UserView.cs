using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class UserView
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int EmpId { get; set; }
        public string FullName { get; set; }
        public string Role {  get; set; }   
        public string Department { get; set; }
        public string Designation { get; set; }
        public bool IsDuplicate { get; set; } = false;
    }
}
