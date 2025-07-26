using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using System.Reflection.Emit;

namespace DataAccess.AddDbContext
{
    public sealed class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContext):base(dbContext) {
        
        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<MeetingTimeLimit> MeetingTimeLimits { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .HasOne(u=>u.AppUser)
                .WithMany(b=>b.Bookings)
                .HasForeignKey(b=>b.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<AppUser>()
                .HasOne(u => u.Designation)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DesignationId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

   
