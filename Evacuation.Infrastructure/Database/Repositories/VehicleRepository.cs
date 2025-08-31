using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class VehicleRepository : GenericRepository<VehicleEntity>, IVehicleRepository
    {
        public VehicleRepository(DataContext context) : base(context)
        {
        }
    }
}
