using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using DataAccess.MapperProfile;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service
{
    public static class DependencyInjection
    {
        public static void RegisterServiceLayerDependencies(this IServiceCollection services)
        {
           
            services.AddScoped<IRoomService,RoomService>(); 
            services.AddScoped<IBookingService,BookingService>();   
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookingService, BookingService>();  
            services.AddScoped<IAdminService, AdminService>();  
            services.AddScoped<IDepartmentService, DepartmentService>();    
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
        }
    }
}
