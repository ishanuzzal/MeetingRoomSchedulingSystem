using AutoMapper;
using DataAccess.Attributes;
using DataAccess.CustomException;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.ViewModel.Booking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    [TraceActivity]
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;   
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMeetingTimeLimitRepository _meetingTimeLimitRepository;   
        private readonly UserManager<AppUser> _userManager;
        public BookingService(IBookingRepository bookingRepository,
            IMapper mapper, IMeetingTimeLimitRepository meetingTimeLimitRepository, UserManager<AppUser> _userManager,
            ILogger<BookingService> logger, IHttpContextAccessor httpContextAccessor, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;   
            _meetingTimeLimitRepository = meetingTimeLimitRepository;   
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task AddByAdminAsync(AddBookingAdminView addBookingView)
        {
            try
            {
                var BookingSlotExist = await _bookingRepository.CheckBookingExistAsync(e =>
                e.RoomId == addBookingView.RoomId  && e.IsDeleted == false && e.Status != "Rejected" && addBookingView.StartTime < e.EndTime && addBookingView.EndTime > e.StartTime
                );

                if (BookingSlotExist==true)
                {
                    throw new NoEmptySlotException("Already exist booking on that time frame");
                }

                var SameTimeFrameBookingExist = await _bookingRepository.CheckBookingExistAsync(e =>
                    e.AppUser.Id == addBookingView.AppUserId && e.IsDeleted==false && e.Status != "Rejected" && addBookingView.StartTime < e.EndTime && addBookingView.EndTime > e.StartTime
                );

                if (SameTimeFrameBookingExist==true)
                {
                    throw new SameTimeFrameBookingException("Already booked in another room on that time slot");
                }

                var UserRoomEntity = await _roomRepository.GetAsync(e=>e.Id==addBookingView.RoomId);

                if(UserRoomEntity.MinPersonLimit>addBookingView.NumberOfParticipation || UserRoomEntity.Capacity < addBookingView.NumberOfParticipation)
                {
                    throw new RoomPersonRangeException("The Participations must be between min person limit and capacity of the room");
                }

                var MeetingTimeLimit = await _meetingTimeLimitRepository.GetTimeLimitAsync();

                var meetingRequestTimeRange = (addBookingView.EndTime - addBookingView.StartTime).TotalMinutes;

                if (!(meetingRequestTimeRange <= MeetingTimeLimit.MaximumMinuteTime && meetingRequestTimeRange >= MeetingTimeLimit.MinimumMinuteTime))
                {
                    throw new MeetingTimeLimitException($"The meeting Time Have to be between {MeetingTimeLimit.MinimumMinuteTime} - {MeetingTimeLimit.MaximumMinuteTime} mins");
                }

                var doMeetingTimePass = addBookingView.StartTime<DateTime.Now || addBookingView.EndTime<DateTime.Now;

                if (doMeetingTimePass==true) {
                    throw new PassTimeBookingException("Booking time must be greater than current date and time");
                }

                var model = _mapper.Map<Booking>(addBookingView);
                _bookingRepository.AddAsync(model);
                await _bookingRepository.Commit();
            }
            catch (NoEmptySlotException)
            {
                throw;
            }
            catch (SameTimeFrameBookingException)
            {
                throw;
            }
            catch(RoomPersonRangeException)
            {
                throw;
            }
            catch (MeetingTimeLimitException)
            {
                throw;
            }
            catch(PassTimeBookingException)
            {
                throw;   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
        public async Task AddByUserAsync(AddBookingUserView addBookingUserView)
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    throw new AuthenticationException("User is Not authenticated");
                }

                addBookingUserView.AppUserId = user.Id;
                var bookingSlotExist = await _bookingRepository.CheckBookingExistAsync(e =>
                e.RoomId == addBookingUserView.RoomId && e.IsDeleted == false && e.Status != "Rejected" &&
                addBookingUserView.StartTime < e.EndTime && addBookingUserView.EndTime > e.StartTime
                );

                if (bookingSlotExist==true)
                {
                    throw new NoEmptySlotException("Already exist booking on that time frame");
                }

                var SameTimeFrameBookingExist = await _bookingRepository.CheckBookingExistAsync(e =>
                    e.AppUser.Id == addBookingUserView.AppUserId && e.IsDeleted == false && e.Status != "Rejected" &&
                    addBookingUserView.StartTime < e.EndTime && addBookingUserView.EndTime > e.StartTime
                );

                if (SameTimeFrameBookingExist)
                {
                    throw new SameTimeFrameBookingException("Already booked in another room on that time slot");
                }

                var UserRoomEntity = await _roomRepository.GetAsync(e => e.Id == addBookingUserView.RoomId);

                if (UserRoomEntity.MinPersonLimit > addBookingUserView.NumberOfParticipation || UserRoomEntity.Capacity < addBookingUserView.NumberOfParticipation)
                {
                    throw new RoomPersonRangeException("The Participations must be between min person limit and capacity of the room");
                }

                var MeetingTimeLimit = await _meetingTimeLimitRepository.GetTimeLimitAsync();

                var meetingRequestTimeRange = (addBookingUserView.EndTime - addBookingUserView.StartTime).TotalMinutes;

                if (!(meetingRequestTimeRange <= MeetingTimeLimit.MaximumMinuteTime && meetingRequestTimeRange >= MeetingTimeLimit.MinimumMinuteTime))
                {
                    throw new MeetingTimeLimitException($"The meeting Time Have to be between {MeetingTimeLimit.MinimumMinuteTime} - {MeetingTimeLimit.MaximumMinuteTime} mins");
                }

                var doMeetingTimePass = addBookingUserView.StartTime < DateTime.Now || addBookingUserView.EndTime < DateTime.Now;

                if (doMeetingTimePass == true)
                {
                    throw new PassTimeBookingException("Booking time must be greater than current date and time");
                }

                var model = _mapper.Map<Booking>(addBookingUserView);
                _bookingRepository.AddAsync(model);
                 await _bookingRepository.Commit();
            }
            catch (NoEmptySlotException)
            {
                throw;
            }
            catch (SameTimeFrameBookingException)
            {
                throw;
            }
            catch (RoomPersonRangeException)
            {
                throw;   
            }
            catch (MeetingTimeLimitException)
            {
                throw;
            }
            catch (PassTimeBookingException)
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
                var booking = await _bookingRepository.GetAsync(e => e.Id == id && e.Status == "Pending"); 

                if (booking == null)
                {
                    throw new NullReferenceException("No booking exist");
                }

                _bookingRepository.DeleteBookingAsync(booking);
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
        public async Task<List<ShowBookingView>> GetAllPendingBookingsAsync()
        {
            try
            {
                var PendingBookingList = await _bookingRepository.GetListBookingsWithUser(e => e.IsDeleted == false && e.Status == "Pending");

                if (PendingBookingList == null)
                {

                    return new List<ShowBookingView>();
                }

                var ShowBookingViewList = new List<ShowBookingView>();

                foreach (var booking in PendingBookingList)
                {
                    var showBookingView = _mapper.Map<ShowBookingView>(booking);
                    showBookingView.UserEmail = booking.AppUser.Email!;
                    ShowBookingViewList.Add(showBookingView);
                }

                return ShowBookingViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<ShowBookingView>> GetAllAcceptedBookingsAsync()
        {
            try
            {
                var AcceptedBookingList = await _bookingRepository.GetListBookingsWithUser(e => e.IsDeleted == false && e.Status == "Approved");

                if (AcceptedBookingList== null)
                {

                    return new List<ShowBookingView>();
                }

                var ShowBookingViewList = new List<ShowBookingView>();

                foreach (var booking in AcceptedBookingList)
                {
                    var showBookingView = _mapper.Map<ShowBookingView>(booking);
                    showBookingView.UserEmail = booking.AppUser.Email!;
                    ShowBookingViewList.Add(showBookingView);
                }

                return ShowBookingViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<ShowBookingView>> GetAllRejectedBookingsAsync()
        {
            try
            {
                var AcceptedBookingList = await _bookingRepository.GetListBookingsWithUser(e => e.IsDeleted == false && e.Status == "Rejected");

                if (AcceptedBookingList == null)
                {

                    return new List<ShowBookingView>();
                }

                var ShowBookingViewList = new List<ShowBookingView>();

                foreach (var booking in AcceptedBookingList)
                {
                    var showBookingView = _mapper.Map<ShowBookingView>(booking);
                    showBookingView.UserEmail = booking.AppUser.Email!;
                    ShowBookingViewList.Add(showBookingView);
                }

                return ShowBookingViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<UpdateBookingView> GetAsync(int id)
        {
            try
            {
                var model = await _bookingRepository.GetAsync(e => e.Id == id);

                if (model == null)
                {
                    throw new NullReferenceException("No booking found");
                }

                var showBooking = _mapper.Map<UpdateBookingView>(model);

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
        public async Task<List<ShowBookingView>> GetListAsync()
        {
            try
            {
                var BookingList = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.IsDeleted == false);

                if (BookingList == null)
                {

                    return new List<ShowBookingView>();
                }

                var ShowBookingViewList = new List<ShowBookingView>();
                var loggedInUserEmail = _httpContextAccessor.HttpContext.User.Identity.Name;

                foreach (var booking in BookingList)
                {
                    var showBookingView = _mapper.Map<ShowBookingView>(booking);
                    showBookingView.UserEmail = booking.AppUser.Email!;
                    showBookingView.IsOwner = showBookingView.UserEmail == loggedInUserEmail;
                    showBookingView.ColorCode = booking.Room.ColorCode;
                    ShowBookingViewList.Add(showBookingView);
                }

                return ShowBookingViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<ShowBookingView>> GetAllActiveBookingsAsync(DateTime start, DateTime end)
        {
            try
            {
                var BookingList = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.IsDeleted == false && e.Status != "Rejected" && e.StartTime.Date >= start.Date && e.EndTime.Date <= end.Date);

                if (BookingList == null)
                {

                    return new List<ShowBookingView>();
                }

                var ShowBookingViewList = new List<ShowBookingView>();
                var loggedInUserEmail = _httpContextAccessor.HttpContext.User.Identity.Name;

                foreach (var booking in BookingList)
                {
                    var showBookingView = _mapper.Map<ShowBookingView>(booking);
                    showBookingView.UserEmail = booking.AppUser.Email!;
                    showBookingView.IsOwner = showBookingView.UserEmail == loggedInUserEmail;
                    showBookingView.ColorCode = booking.Room.ColorCode;
                    ShowBookingViewList.Add(showBookingView);
                }

                return ShowBookingViewList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<ShowBookingView> GetPendingBookingAsync(int id)
        {
            try
            {
                var model = await _bookingRepository.GetAsync(e => e.Id == id && e.Status=="Pending");

                if (model == null)
                {
                    throw new NullReferenceException("No booking found");
                }

                var showBooking = _mapper.Map<ShowBookingView>(model);

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
        public async Task UpdateByAdminAsync(int id, AddBookingAdminView addBookingView)
        {
            try
            {

                var bookingModel = await _bookingRepository.GetAsync(e=>e.Id==id && e.Status=="Pending" && e.IsDeleted==false);

                if (bookingModel == null)
                {
                    throw new NullReferenceException("Model with that id not found or it is not in pending state");
                }

                var BookingExist = await _bookingRepository.CheckBookingExistAsync(e =>
                e.Id!=bookingModel.Id && e.RoomId == addBookingView.RoomId && e.IsDeleted == false && e.Status != "Rejected" && addBookingView.StartTime < e.EndTime && addBookingView.EndTime > e.StartTime
                );

                if (BookingExist==true)
                {
                    throw new NoEmptySlotException("Already exist booking on that time frame");
                }

                var SameTimeFrameBookingExist = await _bookingRepository.CheckBookingExistAsync(e =>
                    e.Id != bookingModel.Id && e.AppUser.Id == addBookingView.AppUserId && e.IsDeleted == false && e.Status != "Rejected" 
                    && e.StartTime.Date == addBookingView.StartTime.Date  && addBookingView.StartTime < e.EndTime && addBookingView.EndTime > e.StartTime
                );

                if (SameTimeFrameBookingExist)
                {
                    throw new SameTimeFrameBookingException("Already booked in another room on that time slot");
                }

                var UserRoomEntity = await _roomRepository.GetAsync(e => e.Id == addBookingView.RoomId);

                if (UserRoomEntity.MinPersonLimit > addBookingView.NumberOfParticipation || UserRoomEntity.Capacity < addBookingView.NumberOfParticipation)
                {
                    throw new RoomPersonRangeException("The Participations must be between min person limit and capacity of the room");
                }

                var MeetingTimeLimit = await _meetingTimeLimitRepository.GetTimeLimitAsync();

                var meetingRequestTimeRange = (addBookingView.EndTime - addBookingView.StartTime).TotalMinutes;

                if (!(meetingRequestTimeRange <= MeetingTimeLimit.MaximumMinuteTime && meetingRequestTimeRange >= MeetingTimeLimit.MinimumMinuteTime))
                {
                    throw new MeetingTimeLimitException($"The meeting Time Have to be between {MeetingTimeLimit.MinimumMinuteTime} - {MeetingTimeLimit.MaximumMinuteTime} mins");
                }

                var doMeetingTimePass = addBookingView.StartTime <DateTime.Now || addBookingView.EndTime < DateTime.Now;

                if (doMeetingTimePass == true)
                {
                    throw new PassTimeBookingException("Booking time must be greater than current date and time");
                }

                _mapper.Map(addBookingView,bookingModel);   
                _bookingRepository.UpdateAsync(bookingModel);
                await _bookingRepository.Commit();
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (NoEmptySlotException)
            {
                throw;
            }
            catch (SameTimeFrameBookingException)
            {
                throw;
            }
            catch (RoomPersonRangeException)
            {
                throw;
            }
            catch (MeetingTimeLimitException)
            {
                throw;
            }
            catch (PassTimeBookingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task UpdateByUserAsync(int id, AddBookingUserView addBookingUserView)
        {
            try
            {
                var bookingModel = await _bookingRepository.GetAsync(e => e.Id == id && e.Status == "Pending" && e.IsDeleted == false);

                if (bookingModel == null)
                {
                    throw new NullReferenceException("Model with that id not found or it is not in pending state");
                }

                var EmptySlotExist = await _bookingRepository.CheckBookingExistAsync(e =>
                e.Id != bookingModel.Id && e.RoomId == addBookingUserView.RoomId  &&
                e.IsDeleted == false && e.Status != "Rejected" && addBookingUserView.StartTime < e.EndTime && addBookingUserView.EndTime > e.StartTime
                );

                if (EmptySlotExist)
                {
                    throw new NoEmptySlotException("Already exist booking on that time frame");
                }

                var SameTimeFrameBookingExist = await _bookingRepository.CheckBookingExistAsync(e =>
                    e.Id != bookingModel.Id && e.AppUser.Id == addBookingUserView.AppUserId && e.IsDeleted == false && e.Status != "Rejected" && e.StartTime.Date == addBookingUserView.StartTime.Date && addBookingUserView.StartTime < e.EndTime && addBookingUserView.EndTime > e.StartTime
                );

                if (SameTimeFrameBookingExist)
                {
                    throw new SameTimeFrameBookingException("Already booked in another room on that time slot");
                }

                var UserRoomEntity = await _roomRepository.GetAsync(e => e.Id == addBookingUserView.RoomId);

                if (UserRoomEntity.MinPersonLimit > addBookingUserView.NumberOfParticipation || UserRoomEntity.Capacity < addBookingUserView.NumberOfParticipation)
                {
                    throw new RoomPersonRangeException("The Participations must be between min person limit and capacity of the room");
                }

                var MeetingTimeLimit = await _meetingTimeLimitRepository.GetTimeLimitAsync();

                var meetingRequestTimeRange = (addBookingUserView.EndTime - addBookingUserView.StartTime).TotalMinutes;

                if (!(meetingRequestTimeRange <= MeetingTimeLimit.MaximumMinuteTime && meetingRequestTimeRange >= MeetingTimeLimit.MinimumMinuteTime))
                {
                    throw new MeetingTimeLimitException($"The meeting Time Have to be between {MeetingTimeLimit.MinimumMinuteTime} - {MeetingTimeLimit.MaximumMinuteTime} mins");
                }

                var doMeetingTimePass = addBookingUserView.StartTime < DateTime.Now || addBookingUserView.EndTime < DateTime.Now;

                if (doMeetingTimePass == true)
                {
                    throw new PassTimeBookingException("Booking time must be greater than current date and time");
                }

                _mapper.Map(bookingModel, addBookingUserView);   
                _bookingRepository.UpdateAsync(bookingModel);
                await _bookingRepository.Commit();
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (NoEmptySlotException)
            {
                throw;
            }
            catch (SameTimeFrameBookingException)
            {
                throw;
            }
            catch (RoomPersonRangeException)
            {
                throw;
            }
            catch (MeetingTimeLimitException)
            {
                throw;
            }
            catch (PassTimeBookingException)
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
