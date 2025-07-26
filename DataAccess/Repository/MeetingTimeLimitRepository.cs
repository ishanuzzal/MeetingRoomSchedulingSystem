using DataAccess.AddDbContext;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class MeetingTimeLimitRepository : GenericRepository<MeetingTimeLimit>, IMeetingTimeLimitRepository
    {
        public AppDbContext _context;
        public MeetingTimeLimitRepository(AppDbContext context):base(context) 
        {
            _context = context; 
        }
        public async Task<MeetingTimeLimit> GetTimeLimitAsync()
        {
            var model =  await _context.MeetingTimeLimits.FirstOrDefaultAsync();

            return model;
        }
    }
}
