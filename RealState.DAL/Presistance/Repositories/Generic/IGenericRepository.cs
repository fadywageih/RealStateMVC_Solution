using RealState.DAL.Models;

namespace RealState.DAL.Presistance.Repositories.Generic
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        Task<IEnumerable<T>> GetAllAsync(bool WithAsNoTracking = true);
        IQueryable<T> GetAllAsQuerable();
        Task<T?> GetByIdAsync(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
