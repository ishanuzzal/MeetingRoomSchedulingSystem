using DataAccess.AddDbContext;
using DataAccess.Attributes;
using DataAccess.CustomModel;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.ViewModel.PaginatedViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    [TraceActivity]
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly AppDbContext _context;
        public BookingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> CheckBookingExistAsync(Expression<Func<Booking, bool>> filter = null)
        {
            var checkedBooking = await _context.Bookings.AnyAsync(filter);

            return checkedBooking;
        }
        public async Task<bool> CheckUserHaveAPendingMeeting(Expression<Func<Booking, bool>> filter = null)
        {
            var userHaveAPendingMeeting = await _context.Bookings.AnyAsync(filter);

            return userHaveAPendingMeeting;
        }
        public void DeleteBookingAsync(Booking booking)
        {
            booking.IsDeleted = true;
            _context.Bookings.Update(booking);
        }
        public async Task<List<AllBookingDetails>> GetAllBookingDetailsAsync(Expression<Func<AllBookingDetails, bool>> filter = null)
        {
            IQueryable<AllBookingDetails> bookingDetails = _context.Bookings.Select(e => new AllBookingDetails
            {
                Booking = e,
                RoomName = "Level " + e.Room.Floor.ToString() + e.Room.Name,
                UserName = e.AppUser.UserName!
            });

            if (filter != null)
            {
                bookingDetails = bookingDetails.Where(filter);
            }

            return await bookingDetails.ToListAsync();
        }
        public async Task<List<Booking>> GetAllBookingWithEagerLoading(Expression<Func<Booking, bool>> filter = null)
        {
            IQueryable<Booking> query = _context.Bookings.Include(e => e.AppUser).Include(e => e.Room).AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
        public async Task<PaginatedBookingModel> GetAllPaginatedBookingDetailsAsync(Expression<Func<Booking, bool>> filter = null, string orderBy = "asc", int currentPage = 1)
        {
            IQueryable<Booking> query = _context.Bookings.Include(e=>e.AppUser).Include(e => e.Room).AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            int pageSize = 5;
            var totalBookings = await query.CountAsync();
            query = query.Skip((currentPage-1)*pageSize).Take(pageSize);
            
            switch(orderBy)
            {
                case "date_asc":
                    query = query.OrderBy(b => b.StartTime);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(b => b.StartTime); 
                    break;
                case "RequestedDate_asc":
                    query = query.OrderBy(b=>b.UpdatedAt??b.CreatedAt);
                    break;
                case "RequestedDate_desc":
                    query = query.OrderByDescending(b => b.UpdatedAt??b.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(b => b.UpdatedAt ?? b.CreatedAt);
                    break;
            }

            var allBookingList = await query.ToListAsync();
            var paginatedBookingModel = new PaginatedBookingModel()
            {
                bookings = allBookingList,
                TotalBooking = totalBookings,
            };

            return paginatedBookingModel;
        }
        public async Task<List<Booking>> GetListBookingsWithUser(Expression<Func<Booking, bool>> filter = null)
        {
            IQueryable<Booking> query = _context.Bookings.Include(e => e.AppUser).AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
    }

}