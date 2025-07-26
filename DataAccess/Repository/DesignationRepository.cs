using DataAccess.AddDbContext;
using DataAccess.Entities;
using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class DesignationRepository:GenericRepository<Designation>, IDesignationRepository
    {
        private readonly AppDbContext _context;
        public DesignationRepository(AppDbContext context):base(context)
        {
            _context = context;
        }
    }
}
