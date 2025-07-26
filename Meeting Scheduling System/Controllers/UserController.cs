using DataAccess.AddDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Meeting_Scheduling_System.Controllers
{
    [Authorize(Roles ="USER")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly ILogger<UserController> _logger;   

        public UserController(IUserService userService, ILogger<UserController> logger, IRoomService roomService)
        {
            _userService = userService;
            _roomService = roomService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AUserAllBookings()
        {
            try
            {
                var getAllUserBooking = await _userService.GetAllBookingListAsync();

                return View(getAllUserBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AUserAllPendingBookings()
        {
            try
            {
                var getAllUserBooking = await _userService.GetAllPendingBookingListAsync();
                return View(getAllUserBooking);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AUserAllAcceptedBookings()
        {
            try
            {
                var getAllUserBooking = await _userService.GetAllAcceptedBookingListAsync();

                return View(getAllUserBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AUserAllRejectedBookings()
        {
            try
            {
                var getAllUserBooking = await _userService.GetAllRejectedBookingListAsync();
                return View(getAllUserBooking);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllRooms()
        {
            try
            {
                var allRooms = await _roomService.GetListAsync();

                return View(allRooms);  
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);

                return View();
            }
        }
    }
}
