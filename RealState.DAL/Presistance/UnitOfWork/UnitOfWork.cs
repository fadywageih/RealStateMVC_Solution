using RealState.DAL.Presistance.Data;
using RealState.DAL.Presistance.Repositories.Property;
using RealState.DAL.Presistance.Repositories.Users;

namespace RealState.DAL.Presistance.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public IUserRepository UserRepository
        {
            get
            { return new UserRepository(_applicationDBContext); }
        }
        public IPropertyRepository PropertyRepository
        {
            get
            { return new PropertyRepository(_applicationDBContext); }
        }


        public UnitOfWork(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<int> CompleteAsync()
        {
            return await _applicationDBContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _applicationDBContext.DisposeAsync();
        }
    }
}