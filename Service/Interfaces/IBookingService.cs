using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBookingService
    {
        public Task AddByAdminAsync(AddBookingAdminView addBookingView);
        public Task AddByUserAsync(AddBookingUserView addBookingUserView);
        public Task<List<ShowBookingView>> GetListAsync();
        public Task<List<ShowBookingView>> GetAllActiveBookingsAsync(DateTime start, DateTime end);
        public Task<List<ShowBookingView>> GetAllPendingBookingsAsync();
        public Task DeleteBookingAsync(int id);
        public Task<UpdateBookingView> GetAsync(int id);
        public Task<ShowBookingView> GetPendingBookingAsync(int id);
        public Task<List<ShowBookingView>> GetAllAcceptedBookingsAsync();
        public Task<List<ShowBookingView>> GetAllRejectedBookingsAsync();
        public Task UpdateByAdminAsync(int id, AddBookingAdminView addBookingView);
        public Task UpdateByUserAsync(int id, AddBookingUserView addBookingUserView);
    }

}
