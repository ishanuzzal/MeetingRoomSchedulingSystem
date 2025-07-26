using DataAccess.AddDbContext;
using DataAccess.Attributes;
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
    [TraceActivity("RoomRepository")]
    public class RoomRepository: GenericRepository<Room>, IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context):base(context) {
            _context = context;
        }
        public override async Task<List<Room>> GetListAsync(Expression<Func<Room, bool>> filter = null)
        {
            var allRoomQuery = _context.Rooms.AsNoTracking();

            if (filter != null)
            {
                allRoomQuery = allRoomQuery.Where(filter);  
            }

            var allRoomList =  await allRoomQuery.OrderBy(e => e.Floor).ThenBy(e => e.Name).ToListAsync();

            return allRoomList;
        }
    }
}
