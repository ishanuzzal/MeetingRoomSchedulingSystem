using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPasswordResetService
    {
        public Task AddAsync(string userId);
        public Task<List<AdminPasswordResetView>> GetListAsync();
        public Task<AdminPasswordResetView> GetAsync(int roomId);
        public Task ApprovedPendingPasswordRequestAsync(int requestId, string temporaryPassword);
        public Task RejectedPendingPasswordRequestAsync(int requestId);
        public Task DeleteAsync(int roomId);
    }
}
