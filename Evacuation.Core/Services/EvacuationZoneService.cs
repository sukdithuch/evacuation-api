using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Domain.Entities;

namespace Evacuation.Core.Services
{
    public class EvacuationZoneService : IEvacuationZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EvacuationZoneService(IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<EvacuationZoneResponse>> GetEvacuationZonesAsync()
        {
            var zones = await _unitOfWork.EvacuationZones.GetAllAsync();
            return _mapper.Map<List<EvacuationZoneResponse>>(zones);
        }

        public async Task<List<EvacuationZoneResponse>> GetActiveEvacuationZonesAsync()
        {
            var zones = await _unitOfWork.EvacuationZones.GetAllActiveAsync();
            return _mapper.Map<List<EvacuationZoneResponse>>(zones);
        }

        public async Task<EvacuationZoneResponse> GetEvacuationZoneByIdAsync(int id)
        {
            var existingZone = await _unitOfWork.EvacuationZones.FindByIdAsync(id);
            if (existingZone != null) 
                throw new ArgumentException($"EvacuationZone not found.");

            return _mapper.Map<EvacuationZoneResponse>(existingZone);
        }

        public async Task<EvacuationZoneResponse> CreateEvacuationZoneAsync(EvacuationZoneRequest req)
        {
            var entity = _mapper.Map<EvacuationZoneEntity>(req);
            entity.RemainingPeople = entity.NumberOfPeople;
            var addedZone = await _unitOfWork.EvacuationZones.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvacuationZoneResponse>(addedZone);
        }

        public async Task<EvacuationZoneResponse> UpdateEvacuationZoneAsync(int id, EvacuationZoneRequest req)
        {
            var existingZone = await _unitOfWork.EvacuationZones.FindByIdAsync(id);
            if (existingZone != null) 
                throw new ArgumentException($"EvacuationZone not found.");

            existingZone.Latitude = req.Latitude;
            existingZone.Longitude = req.Longitude;
            existingZone.NumberOfPeople = req.NumberOfPeople;
            existingZone.UrgencyLevel = (ZoneUrgencyLevel)req.UrgencyLevel;
            existingZone = _unitOfWork.EvacuationZones.Update(existingZone);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvacuationZoneResponse>(existingZone);
        }

        public async Task<EvacuationZoneResponse> DeleteEvacuationZoneAsync(int id)
        {
            var existingZone = await _unitOfWork.EvacuationZones.FindByIdAsync(id);
            if (existingZone != null) 
                throw new ArgumentException($"EvacuationZone not found.");

            existingZone = _unitOfWork.EvacuationZones.Remove(existingZone);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EvacuationZoneResponse>(existingZone);
        }
    }
}
