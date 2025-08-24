using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationPlanRepository : GenericRepository<EvacuationPlanEntity>, IEvacuationPlanRepository
    {
        public EvacuationPlanRepository(DataContext context) : base(context)
        {
        }
    }
}
