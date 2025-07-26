using AutoMapper;
using DataAccess.CustomException;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Room;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly ILogger<PasswordResetService> _logger; 
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PasswordResetService(IPasswordResetRepository passwordResetRepository, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor,
            ILogger<PasswordResetService> logger, IMapper mapper, IEmailSenderService emailSenderService)
        {
            _passwordResetRepository = passwordResetRepository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor; 
        }
        public async Task AddAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    throw new NullReferenceException("User Not found");
                }

                var pendingRequestExist = await _passwordResetRepository.CheckHaveAny(e=>e.AppUserId==user.Id && e.Status=="Pending");

                if (pendingRequestExist)
                {
                    throw new HavePendingRequestException("Already have pending password request");
                }

                var PasswordReset = new PasswordResetRequest
                {
                    RequestedTime = DateTime.Now,
                    AppUserId = user.Id,
                    Status = "Pending"
                };

                _passwordResetRepository.AddAsync(PasswordReset);   
                await _passwordResetRepository.Commit();   
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch(HavePendingRequestException)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public Task DeleteAsync(int roomId)
        {
            throw new NotImplementedException();
        }
        public Task<AdminPasswordResetView> GetAsync(int roomId)
        {
            throw new NotImplementedException();
        }
        public async Task<List<AdminPasswordResetView>> GetListAsync()
        {
            try
            {
                var allActivePaswordResetRequest = await _passwordResetRepository.GetAllRequestWithEagerLoading(e => e.Status == "Pending");
                var allAdminPasswordResetView = new List<AdminPasswordResetView>();  

                foreach(var passwordResetRequest  in allActivePaswordResetRequest)
                {
                    var passwordResetView = _mapper.Map<AdminPasswordResetView>(passwordResetRequest);
                    passwordResetView.Email = passwordResetRequest.AppUser.Email!;
                    allAdminPasswordResetView.Add(passwordResetView);
                }

                return allAdminPasswordResetView;   
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            } 
        }
        public async Task RejectedPendingPasswordRequestAsync(int requestId)
        {
            try
            {
                var request = await _passwordResetRepository.GetAsync(e => e.Id == requestId);

                if (request == null)
                {
                    throw new NullReferenceException("no request found");
                }

                _passwordResetRepository.DeleteAsync(request);
                await _passwordResetRepository.Commit();
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
        public async Task ApprovedPendingPasswordRequestAsync(int requestId, string temporaryPassword)
        {
            try
            {
                using(var transaction = await _passwordResetRepository.GetContext().Database.BeginTransactionAsync())
                {
                    try
                    {
                        var request = await _passwordResetRepository.GetAsync(e => e.Id == requestId && e.Status == "Pending");

                        if (request == null)
                        {
                            throw new NullReferenceException("Password request not exist");
                        }

                        var requestedUser = await _userManager.FindByIdAsync(request.AppUserId);

                        if (requestedUser == null)
                        {
                            throw new NullReferenceException("User not exist");
                        }

                        var token = await _userManager.GeneratePasswordResetTokenAsync(requestedUser);
                        var resetPassword = await _userManager.ResetPasswordAsync(requestedUser, token, temporaryPassword);

                        if (resetPassword.Succeeded)
                        {
                            request.ApprovedTime = DateTime.Now;
                            request.Status = "Approved";
                            request.DefaultPassword = temporaryPassword;
                            _passwordResetRepository.UpdateAsync(request);
                            await _passwordResetRepository.Commit(); 
                            await transaction.CommitAsync();
                            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                            await _emailSenderService.SendEmail(requestedUser.Email!, temporaryPassword);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
