using AutoMapper;
using Azure.Core;
using DataAccess.CustomException;
using DataAccess.ViewModel.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Meeting_Scheduling_System.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;
        private readonly IMapper _mapper;
        public RoomController(IRoomService roomService, ILogger<RoomController> logger, IMapper mapper)
        {
            _roomService = roomService;
            _logger = logger;
            _mapper = mapper;   
        }

        [HttpGet("Room/ShowRoom/{qrCodeIdentifier}")]
        [AllowAnonymous]
        public async Task<IActionResult> ShowRoom(string qrCodeIdentifier)
        {
            try
            {
                var room = await _roomService.GetByQrIdentifierAsync(qrCodeIdentifier);     

                if (room == null)
                {
                    throw new NullReferenceException("No room found");
                }

                return View(room);
            }
            catch (NullReferenceException ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddRoom()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoom(AddRoomView addRoomView)
        {
            if (!ModelState.IsValid)
            {
                return View(addRoomView);
            }

            var request = Request;
            var hostAddress = request.Host.ToString();

            try
            {
                await _roomService.AddAsync(addRoomView,hostAddress);

                return RedirectToAction("ViewAllRoom");
            }
            catch (EntityAlreadyExistException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = "Something went wrong";

                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ViewAllRoom()
        {
            try
            {
                var AllRoom = await _roomService.GetListAsync();

                return View(AllRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = "Something went wrong";
            }

            return View();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> EditRoom(int Id)
        {
            try
            {
                var room = await _roomService.GetAsync(Id);

                if(room == null)
                {
                    throw new NullReferenceException("No room found");
                }
                
                var updateRoomView = _mapper.Map<UpdateRoomView>(room);

                return View(updateRoomView);
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;

                return RedirectToAction("ViewAllRoom", "Room");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return View();
            }

        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(UpdateRoomView updateRoomView)
        {
            if (!ModelState.IsValid)
            {

                return View(updateRoomView);
            }

            try
            {
                var hostAddress = Request.Host.ToString();
                await _roomService.UpdateAsync(updateRoomView,hostAddress);

                return RedirectToAction("ViewAllRoom");
            }
            catch (EntityAlreadyExistException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (NullReferenceException)
            {
                TempData["ErrorMessage"] = "No room found";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);  
                TempData["ErrorMessage"] = "Something went wrong";
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteRoom(int Id)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid Operation";

                return RedirectToAction("ViewAllRoom");
            }

            try
            {
                await _roomService.DeleteAsync(Id);
                TempData["message"] = "Deleted Successfully";
            }
            catch (NullReferenceException)
            {
                TempData["error"] = "No room found";
            }
            catch(DeleteingRoomHaveActiveMeetingException)
            {
                TempData["error"] = "Room have active bookings";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("ViewAllRoom");
        }
    }
}
