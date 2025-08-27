using AutoMapper;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Domain.Entities;
using Evacuation.Shared.Utilities;

namespace Evacuation.Core.Services
{
    public class EvacuationPlanService : IEvacuationPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public EvacuationPlanService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<List<EvacuationPlanResponse>> GeneratePlans()
        {
            var plans = new List<EvacuationPlanResponse>();

            var zones = await _unitOfWork.EvacuationZones.GetAllActiveAsync();
            var vehicles = await _unitOfWork.Vehicles.GetAllActiveAsync(); 
            var activePlans = await _unitOfWork.EvacuationPlans.GetAllActiveAsync();

            if (!zones.Any())
                throw new InvalidOperationException("No zones found.");

            if (!vehicles.Any())
                throw new InvalidOperationException("No vehicles found.");

            var clonedZones = zones.Select(z =>
            {
                var clone = _mapper.Map<EvacuationZoneEntity>(z);
                clone.RemainingPeople -= activePlans
                    .Where(p => p.ZoneId.Equals(z.ZoneId))
                    .Sum(p => p.NumberOfPeople);

                clone.RemainingPeople = Math.Max(0, clone.RemainingPeople);
                return clone;
            }).ToList();

            var sortedZones = clonedZones.OrderByDescending(z => z.UrgencyLevel).ToList();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                while (sortedZones.Any(z => z.RemainingPeople > 0) && vehicles.Any(v => v.IsAvailable))
                {
                    foreach (var zone in sortedZones)
                    {
                        if (zone.RemainingPeople <= 0) continue;

                        var vehiclesAvailable = vehicles.Where(v => v.IsAvailable).ToList();
                        var bestVehicle = SelectBestVehicleForZone(vehiclesAvailable, zone);
                        if (bestVehicle == null) break;

                        bestVehicle.IsAvailable = false;
                        _unitOfWork.Vehicles.Update(bestVehicle);

                        int evacuatedCount = Math.Min(zone.RemainingPeople, bestVehicle.Capacity);
                        zone.RemainingPeople -= evacuatedCount;

                        double distance = GeoUtils.HaversineDistanceKm(zone.Latitude, zone.Longitude, bestVehicle.Latitude, bestVehicle.Longitude);
                        double eta = GeoUtils.EstimatedTravelTimeMinutes(distance, bestVehicle.Speed);

                        var planEntity = new EvacuationPlanEntity
                        {
                            ZoneId = zone.ZoneId,
                            VehicleId = bestVehicle.VehicleId,
                            EstimatedArrivalMinutes = Convert.ToInt32(eta),
                            NumberOfPeople = evacuatedCount,
                        };

                        var addedPlan = await _unitOfWork.EvacuationPlans.AddAsync(planEntity);
                        var plan = _mapper.Map<EvacuationPlanResponse>(addedPlan);
                        plans.Add(plan);

                        string key = $"Z:{zone.ZoneId}:STATUS";
                        var cacheStatus = await _cacheService.GetAsync<EvacuationStatusResponse>(key);
                        if (cacheStatus == null)
                        {
                            var status = new EvacuationStatusResponse
                            {
                                ZoneId = zone.ZoneId,
                                TotalEvacuated = 0,
                                RemainingPeople = zone.NumberOfPeople,
                                LastVehicleUsedId = null
                            };
                            await _cacheService.SetAsync(key, status);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            if (!plans.Any())
                throw new InvalidOperationException("No suitable vehicle found for any zone.");

            return plans;
        }

        public async Task ClearPlansAsync()
        {
            var zones = await _unitOfWork.EvacuationZones.GetAllActiveAsync();
            var vehicles = await _unitOfWork.Vehicles.GetAllActiveAsync();
            var plans = await _unitOfWork.EvacuationPlans.GetAllActiveAsync();

            _unitOfWork.EvacuationZones.RemoveAll(zones);
            _unitOfWork.Vehicles.RemoveAll(vehicles);
            _unitOfWork.EvacuationPlans.RemoveAll(plans);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.ClearAllAsync();
        }

        private VehicleEntity SelectBestVehicleForZone(List<VehicleEntity> vehiclesAvailable, EvacuationZoneEntity zone)
        {
            if (!vehiclesAvailable.Any())
                return null;

            VehicleEntity bestVehicle = vehiclesAvailable
                .Select(v =>
                {
                    double distance = GeoUtils.HaversineDistanceKm(zone.Latitude, zone.Longitude, v.Latitude, v.Longitude);
                    double eta = GeoUtils.EstimatedTravelTimeMinutes(distance, v.Speed);
                    int capacityDiff = v.Capacity - zone.RemainingPeople;

                    return new
                    {
                        Vehicle = v,
                        DistanceKm = distance,
                        Eta = eta,
                        CapacityDiff = capacityDiff
                    };
                })
                .Where(x => x.Eta <= 60 &&
                    (x.CapacityDiff <= 0 || 
                    (x.CapacityDiff > 0 && x.CapacityDiff <= zone.RemainingPeople * 0.2)))
                .OrderBy(x => x.DistanceKm)
                .ThenBy(x => x.Eta)
                .ThenBy(x => Math.Abs(x.CapacityDiff))
                .FirstOrDefault()?.Vehicle;

            return bestVehicle; 
        }

        //private List<VehicleEntity> GetVehiclesOrderByDistance(List<VehicleEntity> vehicles, List<EvacuationZoneEntity> zones)
        //{
        //    var vehiclesSort = new List<EvacuationZoneEntity>();
        //    foreach (var vehicle in vehicles)
        //    {

        //    }
        //}
    }
}
