using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Core.Interfaces.Services
{
    public interface IEvacuationZoneService
    {
        Task<List<EvacuationZoneResponse>> GetEvacuationZonesAsync();
        Task<List<EvacuationZoneResponse>> GetActiveEvacuationZonesAsync();
        Task<EvacuationZoneResponse> GetEvacuationZoneByIdAsync(int id);
        Task<EvacuationZoneResponse> CreateEvacuationZoneAsync(EvacuationZoneRequest req);
        Task<EvacuationZoneResponse> UpdateEvacuationZoneAsync(int id, EvacuationZoneRequest req);
        Task<EvacuationZoneResponse> DeleteEvacuationZoneAsync(int id);
    }
}
