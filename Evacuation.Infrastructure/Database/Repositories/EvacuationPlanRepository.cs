using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationPlanRepository : GenericRepository<EvacuationPlanEntity>, IEvacuationPlanRepository
    {
        public EvacuationPlanRepository(DataContext context) : base(context)
        {
        }
    }
}
