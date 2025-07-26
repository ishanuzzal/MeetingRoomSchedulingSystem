using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using DataAccess.AddDbContext;
using DataAccess.MapperProfile;
using DataAccess.Interfaces;
using DataAccess.Repository;
using DataAccess.Interceptors;
using DataAccess.BackgroundServices;


namespace DataAccess
{
    public static class DependencyInjection
    {
        public static void RegisterDataAccessLayerffDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("dbcs"));
                options.AddInterceptors(new TimeAuditableInterceptor());
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true; 
                options.Password.RequiredLength = 8; 
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); 
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IMeetingTimeLimitRepository,MeetingTimeLimitRepository>();    
            services.AddScoped<IDesignationRepository, DesignationRepository>();    
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();  
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

            services.AddHostedService<BookingExpireService>();

        }
    }
}
