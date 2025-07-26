using AutoMapper;
using DataAccess.CustomException;
using DataAccess.CustomModel;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.Repository;
using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Booking;
using DataAccess.ViewModel.PaginatedViewModel;
using DataAccess.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMeetingTimeLimitRepository _meetingTimeLimitRepository;
        private readonly ILogger<AdminService> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AdminService(IBookingRepository bookingRepository, ILogger<AdminService> logger, IMapper mapper, IMeetingTimeLimitRepository meetingTimeLimitRepository,UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _logger = logger;
            _mapper = mapper;
            _meetingTimeLimitRepository = meetingTimeLimitRepository;  
            _userManager = userManager;
        }
        public async Task BookingApproval(int id, string approval)
        {
            try
            {
                var model = await _bookingRepository.GetAsync(e => e.Id == id && e.Status=="Pending");

                if (model == null)
                {
                    throw new NullReferenceException("No booking found");
                }

                model.Status = approval;
                _bookingRepository.UpdateAsync(model);
                await _bookingRepository.Commit();  
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteBookingAsync(int id)
        {
            try
            {
                var model = await _bookingRepository.GetAsync(e => e.Id == id && e.IsDeleted==false);

                if (model == null)
                {
                    throw new NullReferenceException("Booking not found");
                }

                _bookingRepository.DeleteBookingAsync(model);
                await _bookingRepository.Commit();
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<BookingAllDetailsPaginatedView> GetAllBookingsDetailsAsync(int empId = 0, string orderBy = "date_asc", int currentPage = 1)
        {
            try
            {
                var bookingAllDetailsViewList = new List<BookingAllDetailsView>();
                var paginatedBookings = empId != 0 ? await _bookingRepository.GetAllPaginatedBookingDetailsAsync(e=>e.AppUser.EmpId==empId,orderBy,currentPage) :
                                                  await _bookingRepository.GetAllPaginatedBookingDetailsAsync(null,orderBy, currentPage);

                foreach (var bookingDetails in paginatedBookings.bookings)
                {
                    var bookingAllDetailsView = _mapper.Map<BookingAllDetailsView>(bookingDetails);

                    if(bookingDetails.Room != null)
                    {
                        bookingAllDetailsView.Floor = bookingDetails.Room.Floor;
                        bookingAllDetailsView.RoomName = bookingDetails.Room.Name;
                    }

                    if(bookingDetails.AppUser != null)
                    {
                        bookingAllDetailsView.UserName = bookingDetails.AppUser.UserName;
                        bookingAllDetailsView.EmpId = bookingDetails.AppUser.EmpId; 
                    }

                    bookingAllDetailsViewList.Add(bookingAllDetailsView);
                }

                int totalRecords = paginatedBookings.TotalBooking;
                int pageSize = 5;
                int totalPages = (totalRecords + (pageSize - 1)) / pageSize;

                var paginatedBookingList = new BookingAllDetailsPaginatedView()
                {
                    bookingAllDetailsViews = bookingAllDetailsViewList,
                    OrderBy = orderBy,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    EmpId = empId,
                };

                return paginatedBookingList;
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<BookingAllDetailsPaginatedView> GetAllPendingBookingsDetailsAsync(int empId = 0, string orderBy = "date_asc", int currentPage = 1)
        {
            try
            {
                var paginatedBookings = empId != 0 ? await _bookingRepository.GetAllPaginatedBookingDetailsAsync(e => e.AppUser.EmpId == empId && e.Status == "Pending" && e.IsDeleted == false, orderBy, currentPage):
                                                  await _bookingRepository.GetAllPaginatedBookingDetailsAsync(e=>e.Status == "Pending" && e.IsDeleted == false, orderBy, currentPage);

                var bookingAllDetailsViewList = new List<BookingAllDetailsView>();

                foreach (var bookingDetails in paginatedBookings.bookings)
                {
                    var bookingAllDetailsView = _mapper.Map<BookingAllDetailsView>(bookingDetails);

                    if (bookingDetails.UpdatedAt != null)
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.UpdatedAt;
                    }
                    else
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.CreatedAt;
                    }

                    if (bookingDetails.Room != null)
                    {
                        bookingAllDetailsView.Floor = bookingDetails.Room.Floor;
                        bookingAllDetailsView.RoomName = bookingDetails.Room.Name;
                    }

                    if (bookingDetails.AppUser != null)
                    {
                        bookingAllDetailsView.UserName = bookingDetails.AppUser.UserName;
                        bookingAllDetailsView.EmpId = bookingDetails.AppUser.EmpId;
                    }

                    bookingAllDetailsViewList.Add(bookingAllDetailsView);
                }

                int totalRecords = paginatedBookings.TotalBooking;
                int pageSize = 5;
                int totalPages = (totalRecords + (pageSize - 1)) / pageSize;

                var paginatedBookingList = new BookingAllDetailsPaginatedView()
                {
                    bookingAllDetailsViews = bookingAllDetailsViewList,
                    OrderBy = orderBy,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    EmpId = empId,

                };

                return paginatedBookingList;
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<BookingAllDetailsView>> GetAllAcceptedBookingsDetailsAsync()
        {
            try
            {
                var allBookingList = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.Status == "Approved" && e.IsDeleted == false);

                if (allBookingList == null)
                {
                    return new List<BookingAllDetailsView>();
                }

                var bookingAllDetailsViewList = new List<BookingAllDetailsView>();

                foreach (var bookingDetails in allBookingList)
                {
                    var bookingAllDetailsView = _mapper.Map<BookingAllDetailsView>(bookingDetails);

                    if (bookingDetails.Room != null)
                    {
                        bookingAllDetailsView.Floor = bookingDetails.Room.Floor;
                        bookingAllDetailsView.RoomName = bookingDetails.Room.Name;
                    }

                    if (bookingDetails.AppUser != null)
                    {
                        bookingAllDetailsView.UserName = bookingDetails.AppUser.UserName;
                        bookingAllDetailsView.EmpId = bookingDetails.AppUser.EmpId;
                    }

                    if (bookingDetails.UpdatedAt != null)
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.UpdatedAt;
                    }
                    else
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.CreatedAt;
                    }

                    bookingAllDetailsViewList.Add(bookingAllDetailsView);
                }

                return bookingAllDetailsViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<BookingAllDetailsView>> GetAllRejectedBookingsDetailsAsync()
        {
            try
            {
                var allBookingList = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.Status == "Rejected" && e.IsDeleted == false);

                if (allBookingList == null)
                {

                    return new List<BookingAllDetailsView>();
                }

                var bookingAllDetailsViewList = new List<BookingAllDetailsView>();

                foreach (var bookingDetails in allBookingList)
                {
                    var bookingAllDetailsView = _mapper.Map<BookingAllDetailsView>(bookingDetails);

                    if (bookingDetails.UpdatedAt != null)
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.UpdatedAt;
                    }
                    else
                    {
                        bookingAllDetailsView.RequestedAtUtc = bookingDetails.CreatedAt;
                    }

                    if (bookingDetails.Room != null)
                    {
                        bookingAllDetailsView.Floor = bookingDetails.Room.Floor;
                        bookingAllDetailsView.RoomName = bookingDetails.Room.Name;
                    }

                    if (bookingDetails.AppUser != null)
                    {
                        bookingAllDetailsView.UserName = bookingDetails.AppUser.UserName;
                        bookingAllDetailsView.EmpId = bookingDetails.AppUser.EmpId; 
                    }

                    bookingAllDetailsViewList.Add(bookingAllDetailsView);
                }

                return bookingAllDetailsViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<PendingBookingApprovalView> GetPendingBookingApprovalViewAsync(int id)
        {
            try
            {
                var model = await _bookingRepository.GetAsync(e => e.Id == id && e.Status == "Pending");

                if (model == null)
                {
                    throw new NullReferenceException("No booking found");
                }

                var showBooking = _mapper.Map<PendingBookingApprovalView>(model);

                return showBooking;
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<AllUsersView>> GetAllUserListAsync()
        {
            try
            {
                var modelList  = await _userManager.Users.Include(u=>u.Department).Include(u=>u.Designation).ToListAsync(); 

                if (modelList == null)
                {
                    throw new NullReferenceException("No User found");
                }

                var allUserViewList = new List<AllUsersView>();

                foreach (var model in modelList)
                {
                    var userView = _mapper.Map<AllUsersView>(model);

                    if (model.Designation != null) {
                        userView.DesignationName = model.Designation.Name;
                    }

                    if (model.Department != null) {
                        userView.DepartmentName = model.Department.Name;
                    }

                    var roles = await _userManager.GetRolesAsync(model);

                    if(roles != null)
                    {
                        userView.RoleName = roles[0];
                    }

                    allUserViewList.Add(userView);
                }

                return allUserViewList;
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex) {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
        public async Task<MeetingTimeLimit> GetMeetingLimitInMinutesAsync()
        {
            try
            {
                var model = await _meetingTimeLimitRepository.GetAsync();

                if(model == null)
                {
                    throw new NullReferenceException();
                }

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");  
                throw;
            }
        }
        public async Task UpdateTimeLimitAsync(string minTime, string maxTime)
        {
            int minimumTimeInMins = ConvertToMins(minTime);
            int maximumTimeInMins = ConvertToMins(maxTime);

            if(minimumTimeInMins >= maximumTimeInMins)
            {
                throw new MinTimeGreaterThanMaxTimeException("Maximum time must be greater than minimum time");
            }

            try
            {
                var model = await _meetingTimeLimitRepository.GetTimeLimitAsync();
                model.MinimumMinuteTime = minimumTimeInMins;
                model.MaximumMinuteTime = maximumTimeInMins;    
                _meetingTimeLimitRepository.UpdateAsync(model);
                await _meetingTimeLimitRepository.Commit();
            }
            catch (MinTimeGreaterThanMaxTimeException)
            {
                throw;
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private int ConvertToMins(string formattedTime)
        {
            if (formattedTime.Contains("mins"))
            {
                return int.Parse(formattedTime.Replace("mins", ""));
            }
            else
            {
                double timeHrsWithDecimal = double.Parse(formattedTime.Replace("hr",""))*60;

                return (int)timeHrsWithDecimal;
            }
        }
        private string ConvertToString(int formattedTime) {
            TimeSpan time = TimeSpan.FromMinutes(formattedTime);
            var hour =  time.TotalHours;

            if(hour < 1)
            {

                return formattedTime.ToString() + " mins";
            }
            else
            {

                return hour.ToString() + " hr";
            }
        }
        public async Task<SystemSettingView> GetMeetingTimeLimitAsync()
        {
            try
            {
                var model = await _meetingTimeLimitRepository.GetTimeLimitAsync();

                if (model == null)
                {
                    throw new NullReferenceException("TimeLimitNotFound");
                }

                var setting = new SystemSettingView
                {
                    MaximumMeetingTime = ConvertToString(model.MaximumMinuteTime),
                    MinimumMeetingTime = ConvertToString(model.MinimumMinuteTime)
                };    

                return setting; 
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
