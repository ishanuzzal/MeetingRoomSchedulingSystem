using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataAccess.BackgroundServices
{
    public class BookingExpireService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingExpireService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1);
                var delay = nextRun - currentTime;

                await Task.Delay(delay, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                    var prevDay = DateTime.Today.AddDays(-1);
                    var expiredBookings = await bookingRepository.GetListAsync(e =>
                        e.EndTime > prevDay && e.EndTime < DateTime.Now);

                    if (expiredBookings != null && expiredBookings.Count > 0)
                    {
                        foreach (var booking in expiredBookings)
                        {
                            booking.IsDeleted = true;
                            bookingRepository.UpdateAsync(booking);
                        }

                        await bookingRepository.Commit();
                    }
                }
            }
        }
    }
}

