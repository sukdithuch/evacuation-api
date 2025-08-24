using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Core.Interfaces.Infrastructure.Database
{
    public interface IUnitOfWork
    {
        IVehicleRepository Vehicles { get; }
        IEvacuationZoneRepository EvacuationZones { get; }
        IEvacuationPlanRepository EvacuationPlans { get; }
        Task<int> SaveChangesAsync();
    }
}
