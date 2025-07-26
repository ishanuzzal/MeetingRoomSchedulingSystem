using AutoMapper;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.ViewModel.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService : IUserService
    {

        private readonly ILogger<UserService> _logger;  
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager; 
        public UserService(IBookingRepository bookingRepository,IHttpContextAccessor httpContextAccessor, IMapper mapper,
            UserManager<AppUser> userManager,ILogger<UserService> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;    
            _bookingRepository = bookingRepository; 
            _httpContextAccessor = httpContextAccessor; 
        }

        public async Task<List<AUserBookingView>> GetAllBookingListAsync()
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    throw new AuthenticationException("User is Not authenticated");
                }

                var bookingListOfUser = await _bookingRepository.GetAllBookingWithEagerLoading(e=>e.AppUserId==user.Id && e.IsDeleted==false);

                if (bookingListOfUser == null)
                {
                    throw new NullReferenceException("No Booking found");
                }

                var AUserBookingViewList = new List<AUserBookingView>();   

                foreach(var booking in bookingListOfUser)
                {
                    var bookingView = _mapper.Map<AUserBookingView>(booking);

                    if (booking.Room != null)
                    {
                        bookingView.RoomName = booking.Room.Name;
                        bookingView.Floor = booking.Room.Floor;
                    }

                    AUserBookingViewList.Add(bookingView);  
                }

                return AUserBookingViewList;

            }
            catch(AuthenticationException)
            {
                throw;
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch(Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<AUserBookingView>> GetAllPendingBookingListAsync()
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    throw new AuthenticationException("User is Not authenticated");
                }

                var bookingListOfUser = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.AppUserId == user.Id && e.IsDeleted == false && e.Status=="Pending");

                if (bookingListOfUser == null)
                {
                    throw new NullReferenceException("No Booking found");
                }

                var AUserBookingViewList = new List<AUserBookingView>();

                foreach (var booking in bookingListOfUser)
                {
                    var bookingView = _mapper.Map<AUserBookingView>(booking);

                    if (booking.Room != null)
                    {
                        bookingView.RoomName = booking.Room.Name;
                        bookingView.Floor = booking.Room.Floor;
                    }

                    AUserBookingViewList.Add(bookingView);
                }

                return AUserBookingViewList;

            }
            catch (AuthenticationException)
            {
                throw;
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

        public async Task<List<AUserBookingView>> GetAllAcceptedBookingListAsync()
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null) 
                {
                    throw new AuthenticationException("User is Not authenticated");
                } 

                var bookingListOfUser = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.AppUserId == user.Id && e.IsDeleted == false && e.Status == "Approved");
                
                if (bookingListOfUser == null)
                {
                    throw new NullReferenceException("No Booking found");
                }

                var aUserBookingViewList = new List<AUserBookingView>();
                
                foreach (var booking in bookingListOfUser)
                {
                    var bookingView = _mapper.Map<AUserBookingView>(booking);

                    if (booking.Room != null)
                    {
                        bookingView.RoomName = booking.Room.Name;
                        bookingView.Floor = booking.Room.Floor;
                    }

                    aUserBookingViewList.Add(bookingView);
                }

                return aUserBookingViewList;

            }
            catch (AuthenticationException)
            {
                throw;
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
        public async Task<List<AUserBookingView>> GetAllRejectedBookingListAsync()
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    throw new AuthenticationException("User is Not authenticated");
                }

                var bookingListOfUser = await _bookingRepository.GetAllBookingWithEagerLoading(e => e.AppUserId == user.Id && e.IsDeleted == false && e.Status == "Rejected");
                
                if (bookingListOfUser == null)
                {
                    throw new NullReferenceException("No Booking found");
                }

                var aUserBookingViewList = new List<AUserBookingView>();

                foreach (var booking in bookingListOfUser)
                {
                    var bookingView = _mapper.Map<AUserBookingView>(booking);

                    if (booking.Room != null)
                    {
                        bookingView.RoomName = booking.Room.Name;
                        bookingView.Floor = booking.Room.Floor;
                    }

                    aUserBookingViewList.Add(bookingView);
                }

                return aUserBookingViewList;

            }
            catch (AuthenticationException)
            {
                throw;
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

        public async Task<List<UsersMailApiView>> GetUsersEmailList()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users == null)
                {
                    throw new NullReferenceException("No User Found");
                }   

                var usersMailApiViews = new List<UsersMailApiView>();

                foreach (var user in users)
                {
                    var userMailApiView = _mapper.Map<UsersMailApiView>(user);
                    usersMailApiViews.Add(userMailApiView);
                }

                return usersMailApiViews;   
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
