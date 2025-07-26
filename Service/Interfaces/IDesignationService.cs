using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IDesignationService
    {
        public Task<List<Designation>> GetListAsync();
    }
}
