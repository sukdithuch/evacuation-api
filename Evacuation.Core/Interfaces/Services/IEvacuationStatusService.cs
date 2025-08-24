using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;

namespace Evacuation.Core.Interfaces.Services
{
    public interface IEvacuationStatusService
    {
        Task<List<EvacuationStatusResponse>> GetStatusesAsync();
        Task UpdateStatusAsync(EvacuationStatusRequest request);
    }
}
