using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IPasswordResetRepository:IGenericRepository<PasswordResetRequest>
    {
        public Task<List<PasswordResetRequest>> GetAllRequestWithEagerLoading(Expression<Func<PasswordResetRequest,bool>> filter=null);
    }
}
