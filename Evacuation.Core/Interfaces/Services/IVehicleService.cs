using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Core.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<List<VehicleResponse>> GetVehiclesAsync();
        Task<VehicleResponse> GetVehicleByIdAsync(int id);
        Task<VehicleResponse> CreateVehicleAsync(VehicleRequest req);
        Task<VehicleResponse> UpdateVehicleAsync(int id, VehicleRequest req);
        Task<VehicleResponse> DeleteVehicleAsync(int id);
    }
}
