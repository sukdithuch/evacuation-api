using Evacuation.Core.DTOs.Responses;

namespace Evacuation.Core.Interfaces.Services
{
    public interface IEvacuationPlanService
    {
        Task<List<EvacuationPlanResponse>> GeneratePlansAsync();
        Task ClearPlansAsync();
    }
}
