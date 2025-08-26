using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Domain.Entities;
using Evacuation.Shared.Utilities;

namespace Evacuation.Core.Services
{
    public class EvacuationPlanService : IEvacuationPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvacuationPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<List<EvacuationPlanResponse>> GeneratePlans()
        {
            var plans = new List<EvacuationPlanResponse>();

            var zones = await _unitOfWork.EvacuationZones.GetAllActiveAsync();
            var vehicles = await _unitOfWork.Vehicles.GetAllActiveAsync();     
            
            if (!zones.Any())
                throw new ArgumentException("Not found zones.");

            if (!vehicles.Any())
                throw new ArgumentException("Not found vehicles.");            

            //var vehicleIdsUnavailable = new HashSet<int>();
            var sortedZones = zones.OrderByDescending(z => z.UrgencyLevel).ToList();
            while (zones.Any(z => z.RemainingPeople > 0) && vehicles.Any(v => v.IsAvailable))
            {
                foreach (var zone in sortedZones)
                {
                    if (zone.RemainingPeople <= 0) continue;

                    //var vehiclesAvailable = vehicles.Where(v => !vehicleIdsUnavailable.Contains(v.VehicleId)).ToList();
                    var vehiclesAvailable = vehicles.Where(v => v.IsAvailable).ToList();
                    var bestVehicle = SelectBestVehicleForZone(vehiclesAvailable, zone);
                    if (bestVehicle == null) break;

                    bestVehicle.IsAvailable = false;
                    int evacuatedCount = Math.Min(zone.RemainingPeople, bestVehicle.Capacity);
                    zone.TotalEvacuated += evacuatedCount;
                    zone.RemainingPeople -= evacuatedCount;
                    zone.LastVehicleUsedId = bestVehicle.VehicleId;
                    //vehicleIdsUnavailable.Add(bestVehicle.VehicleId);

                    double distance = GeoUtils.HaversineDistanceKm(zone.Latitude, zone.Longitude, bestVehicle.Latitude, bestVehicle.Longitude);
                    double eta = GeoUtils.EstimatedTravelTimeMinutes(distance, bestVehicle.Speed);

                    var plan = new EvacuationPlanResponse
                    {
                        ZoneId = zone.ZoneId,
                        VehicleId = bestVehicle.VehicleId,
                        EstimatedArrivalMinutes = Convert.ToInt32(eta),
                        NumberOfPeople = bestVehicle.Capacity <= zone.NumberOfPeople ? bestVehicle.Capacity : zone.NumberOfPeople
                    };
                    plans.Add(plan);
                }
            }

            return plans;
        }

        public async Task ClearPlansAsync()
        {

        }

        private VehicleEntity SelectBestVehicleForZone(List<VehicleEntity> vehiclesAvailable, EvacuationZoneEntity zone)
        {
            if (vehiclesAvailable.Count == 0)
                return null;

            VehicleEntity bestVehicle = vehiclesAvailable
                .Select(v => new
                {
                    Vehicle = v,
                    DistanceKm = GeoUtils.HaversineDistanceKm(zone.Latitude, zone.Longitude, v.Latitude, v.Longitude)
                })
                .OrderBy(x => x.DistanceKm)
                .ThenBy(x => GeoUtils.EstimatedTravelTimeMinutes(x.DistanceKm, x.Vehicle.Speed))
                .ThenBy(x => Math.Abs(x.Vehicle.Capacity - zone.NumberOfPeople))
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
