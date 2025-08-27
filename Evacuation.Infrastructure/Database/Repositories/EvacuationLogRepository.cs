using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationLogRepository : GenericRepository<EvacuationLogEntity>, IEvacuationLogRepository
    {
        public EvacuationLogRepository(DataContext context) : base(context)
        {
        }
    }
}
