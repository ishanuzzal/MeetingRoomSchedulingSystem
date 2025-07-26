using DataAccess.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        public Task<List<UsersMailApiView>> GetUsersEmailList();
        public Task<List<AUserBookingView>> GetAllBookingListAsync();
        public Task<List<AUserBookingView>> GetAllPendingBookingListAsync();
        public Task<List<AUserBookingView>> GetAllAcceptedBookingListAsync();
        public Task<List<AUserBookingView>> GetAllRejectedBookingListAsync();
    }
}
