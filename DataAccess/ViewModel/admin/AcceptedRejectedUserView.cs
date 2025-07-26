using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.admin
{
    public class AcceptedRejectedUserView
    {
        public List<UserView> AcceptedUserView { get; set; }
        public List<RejectedUserView> RejectedUserView { get; set; }
    }
}
