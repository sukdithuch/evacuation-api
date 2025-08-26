using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Services;

namespace Evacuation.Core.Services
{
    public class EvacuationStatusService : IEvacuationStatusService
    {
        private readonly ICacheService _cacheService;

        public EvacuationStatusService(ICacheService cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<List<EvacuationStatusResponse>> GetStatusesAsync()
        {
            var cached = await _cacheService.GetAsync<string>("ALLKEY");

            return new List<EvacuationStatusResponse>();
        }

        public async Task UpdateStatusAsync(EvacuationStatusRequest request)
        {
            string key = $"Z{request.ZoneId}V{request.VehicleId}:STATUS";
            var status = new EvacuationStatusResponse
            {
                ZoneId = request.ZoneId,
                TotalEvacuated = request.EvacueesMoved,
                LastVehicleUsedId = request.VehicleId,
            };
            await _cacheService.SetAsync(key, status);
        }
    }
}
