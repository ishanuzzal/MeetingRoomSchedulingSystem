using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.User
{
    public class AllUsersView
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int EmpId { get; set; }

        public string DepartmentName { get; set; }

        public string  DesignationName { get; set; }

        public string RoleName { get; set; }
    }
}
