using DataAccess.Attributes;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    [TraceActivity]
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().AddAsync(entity);
        }
        public async Task<bool> CheckHaveAny(Expression<Func<TEntity, bool>> filter)
        {
            var haveAny =  await _dbContext.Set<TEntity>().AnyAsync(filter);

            return haveAny;
        }
        public void DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var model =  await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filter);

            return model;
        }
        public async virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = _dbContext.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query =  query.Where(filter);
            }

            var modelList =  await query.ToListAsync();

            return modelList;   
            
        }
        public void UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
        public DbContext GetContext()
        {
            return _dbContext;
        }
    }
}


