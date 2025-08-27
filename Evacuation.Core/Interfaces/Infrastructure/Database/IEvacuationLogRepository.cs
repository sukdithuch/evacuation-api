using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Core.Interfaces.Infrastructure.Database
{
    public interface IEvacuationLogRepository : IGenericRepository<EvacuationLogEntity>
    {
    }
}
