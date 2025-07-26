using DataAccess.AddDbContext;
using DataAccess.CustomException;
using DataAccess.Entities;
using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.PaginatedViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using CsvHelper;
using DataAccess.ViewModel;
using System.IO;
using System.Globalization;
using CsvHelper.TypeConversion;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Microsoft.SqlServer.Server;
using Meeting_Scheduling_System.Helper;
namespace Meeting_Scheduling_System.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBookingService _bookingService;
        private readonly IAdminService _adminService;
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly IPasswordResetService _passwordResetService;   
        private readonly AppDbContext _appDbContext;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext context, IBookingService bookingService, 
            IAdminService adminService, ILogger<AdminController> logger, IDepartmentService departmentService,IPasswordResetService passwordResetService,
            IDesignationService designationService,AppDbContext appDbContext)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _bookingService = bookingService;
            _adminService = adminService;
            _departmentService = departmentService; 
            _designationService = designationService;
            _appDbContext = appDbContext;
            _passwordResetService = passwordResetService;   
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Calendar()
        {
            return View();
        }
        public async Task<IActionResult> SystemSetting()
        {
            try
            {
                var SystemSetting = await _adminService.GetMeetingTimeLimitAsync();

                return View(SystemSetting);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeMeetingTime()
        {
            var timeIntervals = new List<string>();

            for(var time=TimeSpan.FromMinutes(30); time<=TimeSpan.FromMinutes(60*24); time += TimeSpan.FromMinutes(30))
            {
                if (time.TotalMinutes < 60)
                {
                    timeIntervals.Add($"{time.TotalMinutes} mins");
                }
                else
                {
                    var fixedHours = time.TotalHours;
                    timeIntervals.Add($"{fixedHours} hr");
                }
            }
            try
            {
                var selectedMinMaxTime = await _adminService.GetMeetingTimeLimitAsync();

                var updateMeetingTimeView = new UpdateMeetingTimeView
                {
                    MinimumMeetingTime = timeIntervals,
                    MaximumMeetingTime = timeIntervals,
                    SelectedMaxTime = selectedMinMaxTime.MaximumMeetingTime,
                    SelectedMinTime = selectedMinMaxTime.MinimumMeetingTime
                };

                return View(updateMeetingTimeView);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);   
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMeetingTime(string minTime, string maxTime)
        {
            try
            {
                await _adminService.UpdateTimeLimitAsync(minTime, maxTime);

                return RedirectToAction("SystemSetting");
            }
            catch(MinTimeGreaterThanMaxTimeException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("ChangeMeetingTime");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var listOfUsers = await _adminService.GetAllUserListAsync();

            return View(listOfUsers);
        }

        [HttpPost("BookingApproval/{id}")]
        public async Task<IActionResult> BookingApproval(int id, string approval)
        {
            if (!ModelState.IsValid || (approval != "Approved" && approval != "Rejected")) {
                TempData["error"] = "Invalid request";

                return RedirectToAction("PendingBookings");
            }

            try
            {
                await _adminService.BookingApproval(id, approval);
                TempData["message"] = $"{approval} success";

                return RedirectToAction("PendingBookings");
            }
            catch (NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("PendingBookings", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> PendingBookings(int empId = 0, string orderBy = "date_asc", int currentPage = 1)
        {
            try
            {
                var PendingBookingList = await _adminService.GetAllPendingBookingsDetailsAsync(empId, orderBy, currentPage);

                return View(PendingBookingList);
            }
            catch (NullReferenceException ex)
            {
                TempData["message"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["erroMessage"] = "Something went wrong";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AcceptedBookings()
        {
            try
            {
                var AcceptedBookingList = await _adminService.GetAllAcceptedBookingsDetailsAsync();

                return View(AcceptedBookingList);
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> RejectedBookings()
        {
            try
            {
                var rejectedBookingList = await _adminService.GetAllRejectedBookingsDetailsAsync();

                return View(rejectedBookingList);
            }
            catch (NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllBookingsView(int empId=0, string orderBy = "date_asc", int currentPage = 1)
        {
            try
            {
                var allBookingList = await _adminService.GetAllBookingsDetailsAsync(empId,orderBy,currentPage);

                return View(allBookingList);
            }
            catch(NullReferenceException ex)
            {
                TempData["message"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAcceptedBooking(int id)
        {
            try
            {
                await _adminService.DeleteBookingAsync(id);
                TempData["message"] = "Deleted Successfully";
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("AcceptedBookings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRejectedBooking(int id)
        {
            try
            {
                await _adminService.DeleteBookingAsync(id);
                TempData["message"] = "Deleted Successfully";
            }
            catch (NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("RejectedBookings");
        }

        [HttpGet]
        public IActionResult UploadCsvFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadCsvFile(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            if (file == null || file.Length == 0 || extension!=".csv")
            {
                TempData["error"] = "invalid request";

                return View();
            }

            try
            {
                var allUser = ExtractDataFromCsv(file.OpenReadStream()).ToList();
                HashSet<string> alreadyHaveEmail = new HashSet<string>();

                foreach (var user in allUser)
                {
                    if (alreadyHaveEmail.Contains(user.Email))
                    {
                        user.IsDuplicate = true;
                    }
                    else
                    {
                        alreadyHaveEmail.Add(user.Email);
                    }
                }

                return View(allUser);
            }
            catch (HeaderValidationException ex)
            {
                TempData["error"] = ex.Message; 
            }
            catch (TypeConverterException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (EntityAlreadyExistException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch(InvalidDataException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch(ApplicationException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();
        }
        private IEnumerable<UserView> ExtractDataFromCsv(Stream csvStream)
        {
            var validRecords = new List<UserView>();

            try
            {
                using (var reader = new StreamReader(csvStream))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<UserViewMap>();

                        while (csv.Read())
                        {
                            try
                            {
                                var record = csv.GetRecord<UserView>();
                                validRecords.Add(record);
                            }
                            catch (TypeConverterException ex)
                            {
                                _logger.LogError(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }

            return validRecords;
        }
        public IActionResult CSVRegisteredUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CSVRegisteredUser(List<UserView> userViews)
        {
            AcceptedRejectedUserView acceptedRejectedUserViews = new AcceptedRejectedUserView();
            List<UserView> showUserViews = new List<UserView>();
            List<RejectedUserView> showRejectedUserViews = new List<RejectedUserView>();

            try
            {
                var allDepartment = await _departmentService.GetListAsync();
                var allDesignation = await _designationService.GetListAsync();

                foreach (var user in userViews)
                {
                    RejectedUserView rejectedUserView = new RejectedUserView()
                    {
                        Email = user.Email,
                        Password = user.Password,   
                        EmpId = user.EmpId,
                        FullName = user.FullName,
                        Department = user.Department,
                        Designation = user.Designation,
                        Role = user.Role.ToUpper(),
                        Issues = ""
                    };

                    using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var selectedDepartment = allDepartment.Where(e => e.Name == user.Department).FirstOrDefault();
                            var selectedDesignation = allDesignation.Where(e => e.Name == user.Designation).FirstOrDefault();

                            if (selectedDepartment == null || selectedDesignation == null || user.EmpId == 0 || user.Email.IsNullOrEmpty() || user.Role.IsNullOrEmpty() || user.Password.IsNullOrEmpty())
                            {
                                rejectedUserView.Issues = rejectedUserView.Issues + ", " + "null or invalid values";
                            }

                            var appUser = new AppUser
                            {
                                Email = user.Email,
                                UserName = user.Email,
                                FullName = user.FullName,
                                EmpId = user.EmpId,
                                DepartmentId = selectedDepartment!.Id,
                                DesignationId = selectedDesignation!.Id
                            };

                            var createUserResult = await _userManager.CreateAsync(appUser, user.Password);

                            if(createUserResult.Succeeded)
                            {
                                var roleAddResult = await _userManager.AddToRoleAsync(appUser, user.Role.ToUpper());

                                if (roleAddResult.Succeeded)
                                {
                                    UserView userView = new UserView()
                                    {
                                        Email = appUser.Email,
                                        EmpId = appUser.EmpId,
                                        FullName = appUser.FullName,
                                        Department = selectedDepartment.Name,
                                        Designation = selectedDesignation.Name,
                                        Role = user.Role.ToUpper(),
                                    };
                                    showUserViews.Add(userView);
                                    transaction.Commit();
                                }
                                else
                                {
                                    var uniqueErrors = new HashSet<string>();
                                    foreach (var error in createUserResult.Errors)
                                    {
                                        uniqueErrors.Add(error.Code);
                                    }

                                    foreach (var error in roleAddResult.Errors)
                                    {
                                        uniqueErrors.Add(error.Code);
                                    }

                                    rejectedUserView.Issues = string.Join(" , ", uniqueErrors);

                                    showRejectedUserViews.Add(rejectedUserView);
                                    transaction.Rollback();
                                }
                            }
                            else
                            {
                                var uniqueErrors = new HashSet<string>();

                                foreach (var error in createUserResult.Errors)
                                {
                                    uniqueErrors.Add(error.Code);
                                }

                                rejectedUserView.Issues = string.Join(" , ", uniqueErrors);
                                showRejectedUserViews.Add(rejectedUserView);
                                transaction.Rollback();
                            }   
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex.Message);
                            transaction.Rollback();
                        }
                    }
                }

                acceptedRejectedUserViews.AcceptedUserView = showUserViews;
                acceptedRejectedUserViews.RejectedUserView = showRejectedUserViews;
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return View(acceptedRejectedUserViews);
        }

        [HttpGet]
        public async Task<IActionResult> PendingPasswordRequest()
        {
            try
            {
                var allPendingRequest = await _passwordResetService.GetListAsync(); 

                if (allPendingRequest == null || allPendingRequest.Count()==0)
                {
                    TempData["message"] = "No Request";
                }

                return View(allPendingRequest); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApprovedPasswordPendingRequest(AdminPasswordResetView adminPasswordResetView)
        {

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values
                                  .SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
                TempData["error"] = string.Join("; ", allErrors);

                return RedirectToAction("PendingPasswordRequest");
            }

            try
            {
                await _passwordResetService.ApprovedPendingPasswordRequestAsync(adminPasswordResetView.Id, adminPasswordResetView.DefaultPassword!);
                TempData["message"] = "Email send";

                return RedirectToAction("PendingPasswordRequest");
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("PendingPasswordRequest");   
        }

        [HttpPost]
        public async Task<IActionResult> RejectPasswordPendingRequest(int id)
        {
            try
            {
                await _passwordResetService.RejectedPendingPasswordRequestAsync(id);
                TempData["message"] = "Booking Rejected";

                return RedirectToAction("PendingPasswordRequest");
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("PendingPasswordRequest");
        }

    }
}
