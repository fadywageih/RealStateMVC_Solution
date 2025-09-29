using RealState.DAL.Presistance.Data;
using RealState.DAL.Presistance.Repositories.Generic;

namespace RealState.DAL.Presistance.Repositories.Property
{
    public class PropertyRepository:GenericRepository<Models.Users.Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
