using DataAccess.CustomException;
using DataAccess.Entities;
using DataAccess.ViewModel.Booking;
using DataAccess.ViewModel.Room;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Service.Interfaces;
using Service.Services;

namespace Meeting_Scheduling_System.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            IRoomService roomService,
            IUserService userService,
            ILogger<BookingController> logger,
            IDistributedCache cache)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _userService = userService;
            _logger = logger;
        }


        [HttpGet("GetAllRoomApi")]
        public async Task<IActionResult> GetAllRoomApi()
        {
            try
            {
                List<ShowRoomView> rooms = await _roomService.GetListAsync();
                List<RoomInApi> viewRoom = new List<RoomInApi>();

                foreach (var room in rooms)
                {
                    RoomInApi roomInApi = new RoomInApi();
                    roomInApi.Id = room.Id;
                    roomInApi.Name = "L" + room.Floor.ToString() + "-" + room.RoomName;
                    roomInApi.ColorCode = room.ColorCode;
                    viewRoom.Add(roomInApi);
                }
                
                return Ok(viewRoom);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);

                return NotFound();
            }
        }

        [HttpGet("GetAllUsersEmail")]
        public async Task<IActionResult> GetAllUsersEmail()
        {
            try
            {
                var AllUserEmail = await _userService.GetUsersEmailList();

                return Ok(AllUserEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return NotFound();
            }

        }

        [HttpGet("GetAllActiveBookings")]
        public async Task<IActionResult> GetAllActiveBookings([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {

                var AllBookingView = await _bookingService.GetAllActiveBookingsAsync(start, end);

                return Ok(AllBookingView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }


        [HttpGet("GetAllBooking")]
        public async Task<IActionResult> GetAllBooking()
        {
            try
            {
                var AllBookingView = await _bookingService.GetListAsync();

                return Ok(AllBookingView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("AddBooking")]
        //booking by user
        public async Task<IActionResult> AddBooking(AddBookingUserView addBookingUserView)
        {
            if (!ModelState.IsValid)
            {
                BadRequest("Invalid Data");
            }

            try
            {
                await _bookingService.AddByUserAsync(addBookingUserView);

                return Ok("booking added");
            }
            catch (NoEmptySlotException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SameTimeFrameBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (RoomPersonRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyHavePendingMeetingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MeetingTimeLimitException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PassTimeBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }


        [HttpPost("AddBookingByAdmin")]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookingByAdmin(AddBookingAdminView addBookingAdminView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                await _bookingService.AddByAdminAsync(addBookingAdminView);

                return Ok("booking added");
            }
            catch (NoEmptySlotException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SameTimeFrameBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (RoomPersonRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyHavePendingMeetingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MeetingTimeLimitException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PassTimeBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

        [HttpPut("UpdateBookingByAdmin/{id}")]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingByAdmin([FromRoute]int id, AddBookingAdminView addBookingView)
        {
            if (!ModelState.IsValid)
            {
                BadRequest("Invalid Data");
            }

            try
            {
                await _bookingService.UpdateByAdminAsync(id, addBookingView);

                return Ok("booking updated");
            }
            catch(NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NoEmptySlotException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SameTimeFrameBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyHavePendingMeetingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MeetingTimeLimitException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PassTimeBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

        [HttpPut("UpdateBookingByUser/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingByUser([FromRoute] int id, AddBookingUserView addBookingView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                await _bookingService.UpdateByUserAsync(id, addBookingView);

                return Ok("booking updated");
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NoEmptySlotException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SameTimeFrameBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserAlreadyHavePendingMeetingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MeetingTimeLimitException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PassTimeBookingException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("GetBooking/{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var result = await _bookingService.GetAsync(id);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

        [HttpDelete("DeleteBooking/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                await _bookingService.DeleteBookingAsync(id);

                return Ok("booking deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest("Something went wrong");
            }
        }

    }
}
