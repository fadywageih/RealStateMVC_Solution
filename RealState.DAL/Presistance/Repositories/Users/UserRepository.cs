using RealState.DAL.Models.Users;
using RealState.DAL.Presistance.Data;
using RealState.DAL.Presistance.Repositories.Generic;

namespace RealState.DAL.Presistance.Repositories.Users
{
    public class UserRepository : GenericRepository<User>, IUserRepository    
    {
        public UserRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
        }
    }
}
