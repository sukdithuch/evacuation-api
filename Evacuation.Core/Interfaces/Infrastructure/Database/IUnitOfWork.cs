using System.Data;

namespace Evacuation.Core.Interfaces.Infrastructure.Database
{
    public interface IUnitOfWork
    {
        IVehicleRepository Vehicles { get; }
        IEvacuationZoneRepository EvacuationZones { get; }
        IEvacuationPlanRepository EvacuationPlans { get; }
        IEvacuationLogRepository EvacuationLogs { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
