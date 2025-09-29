using RealState.DAL.Presistance.Repositories.Property;
using RealState.DAL.Presistance.Repositories.Users;

namespace RealState.DAL.Presistance.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IUserRepository UserRepository { get; }
        public IPropertyRepository PropertyRepository { get; }
        Task<int> CompleteAsync();
    }
}
