using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Domain.Entities;

namespace Evacuation.Core.Services
{
    public class EvacuationStatusService : IEvacuationStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public EvacuationStatusService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<List<EvacuationStatusResponse>> GetStatusesAsync()
        {
            var statuses = new List<EvacuationStatusResponse>();
            var keys = await _cacheService.GetAllKeysAsync();
            foreach(var key in keys)
            {
                var status = await _cacheService.GetAsync<EvacuationStatusResponse>(key);
                statuses.Add(status);
            }

            return statuses;
        }

        public async Task UpdateStatusAsync(EvacuationStatusRequest request)
        {
            int zoneId = request.ZoneId;
            var zone = await _unitOfWork.EvacuationZones.FindByIdAsync(zoneId);
            var vehicle = await _unitOfWork.Vehicles.FindByIdAsync(request.VehicleId);
            var plan = (await _unitOfWork.EvacuationPlans.FindByAsync(p => p.ZoneId.Equals(zoneId) && p.VehicleId.Equals(request.VehicleId) && p.Active)).LastOrDefault();

            if (zone == null) 
                throw new ArgumentException($"Zone {zoneId} not found.");
            if (vehicle == null) 
                throw new ArgumentException($"Vehicle {request.VehicleId} not found.");
            if (plan == null) 
                throw new ArgumentException($"Plan not found for zone {zoneId} and vehicle {request.VehicleId}.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                int evacuatedCount = Math.Min(zone.RemainingPeople, request.EvacuatedPeople);
                zone.TotalEvacuated += evacuatedCount;
                zone.RemainingPeople -= evacuatedCount;
                zone.LastVehicleUsedId = request.VehicleId;
                _unitOfWork.EvacuationZones.Update(zone);

                vehicle.IsAvailable = true;
                _unitOfWork.Vehicles.Update(vehicle);

                var log = new EvacuationLogEntity
                {
                    ZoneId = zoneId,
                    VehicleId = request.VehicleId,
                    EstimatedArrivalMinutes = plan.EstimatedArrivalMinutes,
                    NumberOfPeople = plan.NumberOfPeople,
                    EvacuatedPeople = evacuatedCount,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };

                await _unitOfWork.EvacuationLogs.AddAsync(log);

                string key = $"Z:{zoneId}:STATUS";
                var status = new EvacuationStatusResponse
                {
                    ZoneId = zoneId,
                    TotalEvacuated = zone.TotalEvacuated,
                    RemainingPeople = zone.RemainingPeople,
                    LastVehicleUsedId = zone.LastVehicleUsedId,
                };
                await _cacheService.SetAsync(key, status);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
