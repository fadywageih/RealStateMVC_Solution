using Microsoft.EntityFrameworkCore;
using RealState.DAL.Models;
using RealState.DAL.Presistance.Data;

namespace RealState.DAL.Presistance.Repositories.Generic
{
    public class GenericRepository<T>:IGenericRepository<T> where T : ModelBase
    {
        private protected readonly ApplicationDBContext _dbContext;
        public GenericRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool WithAsNoTracking = true)
        {
            if (WithAsNoTracking)
            {
                return await _dbContext.Set<T>().Where(X => !X.IsDeleted).AsNoTracking().ToListAsync();
            }
            return await _dbContext.Set<T>().Where(X => !X.IsDeleted).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                return null;
            }
            return entity;
        }
        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);

        }
        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
            //_dbContext.Set<T>().Remove(entity);
        }
        public IQueryable<T> GetAllAsQuerable()
        {
            return _dbContext.Set<T>().Where(x => !x.IsDeleted);
        }
    }
}
