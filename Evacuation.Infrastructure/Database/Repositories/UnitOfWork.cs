using Evacuation.Core.Interfaces.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IDbContextTransaction? _transaction;

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

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
            await _transaction!.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            await _transaction!.RollbackAsync();
            await _transaction!.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
