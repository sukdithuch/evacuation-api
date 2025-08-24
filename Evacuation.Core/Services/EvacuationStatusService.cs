using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Services;

namespace Evacuation.Core.Services
{
    public class EvacuationStatusService : IEvacuationStatusService
    {

        public EvacuationStatusService()
        {

        }

        public async Task<List<EvacuationStatusResponse>> GetStatusesAsync()
        {
            return new List<EvacuationStatusResponse>();
        }

        public async Task UpdateStatusAsync(EvacuationStatusRequest request)
        {

        }
    }
}
