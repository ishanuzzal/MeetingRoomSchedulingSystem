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
    public class PasswordResetRepository:GenericRepository<PasswordResetRequest>, IPasswordResetRepository
    {
        private readonly AppDbContext _context;
        public PasswordResetRepository(AppDbContext appDbContext):base(appDbContext) 
        {
            _context = appDbContext;   
        }
        public async Task<List<PasswordResetRequest>> GetAllRequestWithEagerLoading(Expression<Func<PasswordResetRequest, bool>> filter = null)
        {
            var query = _context.PasswordResetRequest.Include(e => e.AppUser).AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var result = await query.ToListAsync();

            return result;
        }
    }
}
