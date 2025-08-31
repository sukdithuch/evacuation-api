using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationZoneRepository : GenericRepository<EvacuationZoneEntity>, IEvacuationZoneRepository
    {
        public EvacuationZoneRepository(DataContext context) : base(context)
        {
        }
    }
}
