using DataAccess.Entities;
using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Booking;
using DataAccess.ViewModel.PaginatedViewModel;
using DataAccess.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAdminService
    {
        public Task BookingApproval(int id, string approval);
        public Task<BookingAllDetailsPaginatedView> GetAllBookingsDetailsAsync(int EmpId=0, string orderBy = "date_asc", int currentPage = 1);
        public Task<BookingAllDetailsPaginatedView> GetAllPendingBookingsDetailsAsync(int EmpId = 0, string orderBy = "date_asc", int currentPage = 1);
        public Task<List<BookingAllDetailsView>> GetAllAcceptedBookingsDetailsAsync();
        public Task<List<BookingAllDetailsView>> GetAllRejectedBookingsDetailsAsync();
        public Task<PendingBookingApprovalView> GetPendingBookingApprovalViewAsync(int id);
        public Task<List<AllUsersView>> GetAllUserListAsync();
        public Task DeleteBookingAsync(int id);
        public Task UpdateTimeLimitAsync(string minTime, string maxTime);
        public Task<MeetingTimeLimit> GetMeetingLimitInMinutesAsync();
        public Task<SystemSettingView> GetMeetingTimeLimitAsync(); 
    }
}
