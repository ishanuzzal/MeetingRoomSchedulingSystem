using DataAccess.AddDbContext;
using DataAccess.CustomException;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.ViewModel;
using DataAccess.ViewModel.auth;
using DataAccess.ViewModel.User;
using Meeting_Scheduling_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Serilog;
using Service.Interfaces;
using Service.Services;
using System.Data;
using System.Security.Authentication;

namespace Meeting_Scheduling_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly IPasswordResetService _passwordResetService;
        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IPasswordResetService passwordResetService,
                                IDepartmentService departmentService, IDesignationService designationService, ILogger<AuthController> logger, AppDbContext appDbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _departmentService = departmentService;
            _designationService = designationService;
            _appDbContext = appDbContext;
            _logger = logger;
            _passwordResetService = passwordResetService;
        }
        public async Task<IActionResult> Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("ADMIN"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (roles.Contains("USER"))
                    {
                        return RedirectToAction("index", "User");
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = await _userManager.FindByEmailAsync(login.Email);

                    if (appUser != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                        if (result.Succeeded)
                        {
                            var roles = await _userManager.GetRolesAsync(appUser);

                            if (roles.Contains("ADMIN"))
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Index", "User");
                            }
                        }

                        ModelState.AddModelError("All", "Login Failed: Invalid Email or password");
                    }

                    return View(login);
                }
            }
            catch (Exception ex) {
                ModelState.AddModelError("All", "Internal Server Error");
                _logger.LogError(ex.Message);
            }
               
            return View(login);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }

            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        return RedirectToAction("Login","Auth");
                    }

                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View();
                    }

                    TempData["message"] = "Password successfully changed";
                    await _signInManager.RefreshSignInAsync(user);
                    return View();
                }

                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";

                return View();
            }  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRequest()
        {
            try
            {
                await _passwordResetService.AddAsync(User.Identity.Name);
                TempData["message"] = "Password reset request send. You will get temporary password in your email";
            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (HavePendingRequestException ex)
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
        public IActionResult ResetPasswordRequestWithEmail()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRequestWithEmail(ResetPasswordRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _passwordResetService.AddAsync(model.Email);
                TempData["message"] = "Password reset request send. You will get temporary password in your email";
                return RedirectToAction("Login");
            }
            catch (HavePendingRequestException ex)
            {
                ModelState.AddModelError("All", ex.Message);
            }
            catch (NullReferenceException ex)
            {
                ModelState.AddModelError("All", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("All", "Something went wrong");
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Registration()
        {
            try
            {
                var departments = await _departmentService.GetListAsync();
                var designations = await _designationService.GetListAsync();
                var roles = await _roleManager.Roles.ToListAsync();

                var model = new Registration
                {
                    Roles = roles.Select(r => new SelectListItem()
                    {
                        Value = r.Name,
                        Text = r.Name,
                    }),
                    Departments = departments.Select(d => new SelectListItem()
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                    }),
                    Designations = designations.Select(d => new SelectListItem()
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name,
                    })

                };

                return View(model);
            }
            catch (Exception ex) {
                _logger.LogError($"{ex.Message}", ex);  

                return NotFound();
            }

        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(Registration rg)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    var dropdownData = await SetDataInDropdownRegistration();
                    rg.Departments = dropdownData.Departments;
                    rg.Roles = dropdownData.Roles;
                    rg.Designations = dropdownData.Designations;

                    return View(rg);
                }

                var existingUser = await _userManager.FindByEmailAsync(rg.Email);

                if (existingUser != null)
                {
                    throw new EntityAlreadyExistException("A user with this email already exists.");
                }

                using (var transaction = await _appDbContext.Database.BeginTransactionAsync()){

                    try
                    {
                        var appUser = new AppUser
                        {
                            Email = rg.Email,
                            UserName = rg.Email,
                            FullName = rg.FullName.Trim(),
                            EmpId = rg.EmpId,
                            DepartmentId = rg.SelectedDepartment,
                            DesignationId = rg.SelectedDesignation
                        };

                        var createUserResult = await _userManager.CreateAsync(appUser, rg.Password);

                        if (createUserResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(appUser, rg.SelectedRole);
                            await transaction.CommitAsync();   
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                return RedirectToAction("GetAllUsers", "Admin");
            }
            catch (EntityAlreadyExistException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong.";
            }

            return RedirectToAction("Registration", "Auth");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> EditUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    throw new NullReferenceException("no user found");
                }

                var departments = await _departmentService.GetListAsync();
                var designations = await _designationService.GetListAsync();
                var allRoles = await _roleManager.Roles.ToListAsync();
                var drowdownData = await SetDataInDropdownEditUser(id);

                if (drowdownData == null)
                {
                    throw new NullReferenceException("Resources not found");
                }

                EditUserViewModel editUserViewModel = new EditUserViewModel()
                {
                    Id = id,
                    EmpId = user.EmpId,
                    FullName = user.FullName,
                    Email = user.UserName!,
                    SelectedDepartment = drowdownData.SelectedDepartment,
                    Departments = drowdownData.Departments,
                    SelectedDesignation = drowdownData.SelectedDepartment,
                    Designations = drowdownData.Designations,
                    Roles = drowdownData.Roles,
                    SelectedRole = drowdownData.SelectedRole,
                };

                return View(editUserViewModel);
            }
            catch (NullReferenceException)
            {
                TempData["error"] = "No User found";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("GetAllUsers", "Admin");
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserViewModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var dropdownData = await SetDataInDropdownEditUser(editUserViewModel.Id);
                    editUserViewModel.Designations = dropdownData.Designations;
                    editUserViewModel.Departments = dropdownData.Departments;
                    editUserViewModel.Roles = dropdownData.Roles;
                    return View(editUserViewModel);
                }

                var user = await _userManager.FindByIdAsync(editUserViewModel.Id);

                if (user == null)
                {
                    throw new NullReferenceException("no user found");
                }

                var userWithEmail = await _userManager.Users.Where(u => u.Id!= editUserViewModel.Id && u.Email == editUserViewModel.Email).FirstOrDefaultAsync();

                if (userWithEmail!=null)
                {
                    throw new EntityAlreadyExistException("User with that UserName already Exist");
                }

                using(var transaction = await _appDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        user.Email = editUserViewModel.Email;
                        user.UserName = editUserViewModel.Email;
                        user.EmpId = editUserViewModel.EmpId;
                        user.FullName = editUserViewModel.FullName.Trim();
                        user.DepartmentId = editUserViewModel.SelectedDepartment;
                        user.DesignationId = editUserViewModel.SelectedDesignation;

                        var userRoles = await _userManager.GetRolesAsync(user);
                        var removeRole = await _userManager.RemoveFromRolesAsync(user, userRoles);
                        var roleAddResult = await _userManager.AddToRoleAsync(user, editUserViewModel.SelectedRole);
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded && roleAddResult.Succeeded)
                        {
                            TempData["message"] = "User Information Updated";
                            transaction.Commit();
                            return RedirectToAction("GetAllUsers", "Admin");
                        }
                      
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch(EntityAlreadyExistException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["error"] = "Something went wrong";
            }

            return RedirectToAction("EditUser", "Auth");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Invalid user Id";

                    return RedirectToAction("GetAllUsers", "Admin");
                }

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    throw new NullReferenceException("no user found");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["message"] = "User deleted successfully";

                    return RedirectToAction("GetAllUsers", "Admin");
                }

            }
            catch(NullReferenceException ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }

            return RedirectToAction("GetAllUsers", "Admin");
        }
        private async Task<DropdownModel> SetDataInDropdownRegistration()
        {
            try
            {
                var departments = await _departmentService.GetListAsync();
                var desginations = await _designationService.GetListAsync();
                var allRoles = await _roleManager.Roles.ToListAsync();

                var dropdownItemsObject = new DropdownModel
                {
                    Departments = departments.Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    }),
                    Designations = desginations.Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    }),
                    Roles = allRoles.Select(d => new SelectListItem
                    {
                        Value = d.Name,
                        Text = d.Name
                    }),
                };

                return dropdownItemsObject;
            }
            catch (Exception ex) {
               _logger.LogError($"Error: {ex.Message}");
                throw;
            }
        }
        private async Task<DropdownModel> SetDataInDropdownEditUser(string id)
        {
            try
            {
                var departments = await _departmentService.GetListAsync();
                var desginations = await _designationService.GetListAsync();
                var allRoles = await _roleManager.Roles.ToListAsync();
                var user = await _userManager.FindByIdAsync(id);
                var roles = await _userManager.GetRolesAsync(user!);

                var dropdownItemsObject = new DropdownModel
                {
                    Departments = departments.Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    }),
                    SelectedDepartment = user.DepartmentId,
                    Designations = desginations.Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    }),
                    SelectedDesignation = user.DesignationId,
                    Roles = allRoles.Select(d => new SelectListItem
                    {
                        Value = d.Name,
                        Text = d.Name
                    }),
                    SelectedRole = roles[0]
                };

                return dropdownItemsObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");

                return null;
            }
        }
    }
}
