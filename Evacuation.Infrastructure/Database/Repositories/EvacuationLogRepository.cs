using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationLogRepository : GenericRepository<EvacuationLogEntity>, IEvacuationLogRepository
    {
        public EvacuationLogRepository(DataContext context) : base(context)
        {
        }
    }
}
