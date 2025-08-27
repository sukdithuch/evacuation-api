using Evacuation.Core.Interfaces.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public IVehicleRepository Vehicles { get; }
        public IEvacuationZoneRepository EvacuationZones { get; }
        public IEvacuationPlanRepository EvacuationPlans { get; }
        public IEvacuationLogRepository EvacuationLogs { get; }

        public UnitOfWork(DataContext context, 
            IVehicleRepository vehicles, 
            IEvacuationZoneRepository evacuationZones, 
            IEvacuationPlanRepository evacuationPlans,
            IEvacuationLogRepository evacuationLogs)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Vehicles = vehicles ?? throw new ArgumentNullException(nameof(vehicles));
            EvacuationZones = evacuationZones ?? throw new ArgumentNullException(nameof(evacuationZones));
            EvacuationPlans = evacuationPlans ?? throw new ArgumentNullException(nameof(evacuationPlans));
            EvacuationLogs = evacuationLogs ?? throw new ArgumentNullException(nameof(evacuationLogs));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
