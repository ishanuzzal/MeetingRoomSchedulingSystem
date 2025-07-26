using DataAccess.CustomModel;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        public Task<bool> CheckBookingExistAsync(Expression<Func<Booking, bool>> filter = null);
        public Task<List<Booking>> GetListBookingsWithUser(Expression<Func<Booking, bool>> filter = null);
        public Task<List<Booking>> GetAllBookingWithEagerLoading(Expression<Func<Booking, bool>> filter = null);
        Task<PaginatedBookingModel> GetAllPaginatedBookingDetailsAsync(Expression<Func<Booking, bool>> filter = null, string orderBy = "asc", int currentPage = 1);
        public void DeleteBookingAsync(Booking booking);
        public Task<List<AllBookingDetails>> GetAllBookingDetailsAsync(Expression<Func<AllBookingDetails, bool>> filter = null);
        public Task<bool> CheckUserHaveAPendingMeeting(Expression<Func<Booking, bool>> filter = null);
    }

}
