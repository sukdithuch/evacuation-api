using Evacuation.Domain.Entities;

namespace Evacuation.Core.Interfaces.Infrastructure.Database
{
    public interface IEvacuationPlanRepository : IGenericRepository<EvacuationPlanEntity>
    {
        //Task<List<EvacuationPlanEntity>> FindByZoneAndVehicleAsync(int zoneId, int vehicleId);
    }
}
