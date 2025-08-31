using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Domain.Entities;

namespace Evacuation.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            #region Vehicle
            CreateMap<VehicleRequest, VehicleEntity>();

            CreateMap<VehicleEntity, VehicleResponse>();
            #endregion

            #region EvacuationZone
            CreateMap<EvacuationZoneRequest, EvacuationZoneEntity>()
                .ForMember(dest => dest.UrgencyLevel, opt => opt.MapFrom(src => (ZoneUrgencyLevel)src.UrgencyLevel));

            CreateMap<EvacuationZoneEntity, EvacuationZoneResponse>()
                .ForMember(dest => dest.UrgencyLevel, opt => opt.MapFrom(src => (int)src.UrgencyLevel));

            CreateMap<EvacuationZoneEntity, EvacuationZoneEntity>();
            #endregion

            #region EvacuationPlan
            CreateMap<EvacuationPlanEntity, EvacuationPlanResponse>();
            #endregion
        }
    }
}
