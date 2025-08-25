using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Domain.Entities;

namespace Evacuation.Core.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<VehicleResponse>> GetVehiclesAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
            return _mapper.Map<List<VehicleResponse>>(vehicles);
        }

        public async Task<List<VehicleResponse>> GetActiveVehiclesAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllActiveAsync();
            return _mapper.Map<List<VehicleResponse>>(vehicles);
        }

        public async Task<VehicleResponse> GetVehicleByIdAsync(int id)
        {
            var existingVehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (existingVehicle == null) 
                throw new ArgumentException($"Vehicle not found.");

            return _mapper.Map<VehicleResponse>(existingVehicle);
        }

        public async Task<VehicleResponse> CreateVehicleAsync(VehicleRequest req)
        {
            var entity = _mapper.Map<VehicleEntity>(req);
            var addedVehicle = await _unitOfWork.Vehicles.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleResponse>(addedVehicle);
        }

        public async Task<VehicleResponse> UpdateVehicleAsync(int id, VehicleRequest req)
        {
            var existingVehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (existingVehicle == null)
                throw new ArgumentException($"Vehicle not found.");

            existingVehicle.Capacity = req.Capacity;
            existingVehicle.Type = req.Type;
            existingVehicle.Latitude = req.Latitude;
            existingVehicle.Longitude = req.Longitude;
            existingVehicle.Speed = req.Speed;
            existingVehicle.Status = Enum.Parse<VehicleStatus>(req.Status);
            existingVehicle = _unitOfWork.Vehicles.Update(existingVehicle);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleResponse>(existingVehicle);
        }

        public async Task<VehicleResponse> DeleteVehicleAsync(int id)
        {
            var existingVehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (existingVehicle == null)
                throw new ArgumentException($"Vehicle not found.");

            existingVehicle = _unitOfWork.Vehicles.Remove(existingVehicle);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VehicleResponse>(existingVehicle);
        }
    }
}
