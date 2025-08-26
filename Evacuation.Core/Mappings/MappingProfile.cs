using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            #region Vehicle
            CreateMap<VehicleRequest, VehicleEntity>();
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<VehicleStatus>(src.Status)));

            CreateMap<VehicleEntity, VehicleResponse>();
                //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            #endregion

            #region EvacuationZone
            CreateMap<EvacuationZoneRequest, EvacuationZoneEntity>()
                .ForMember(dest => dest.UrgencyLevel, opt => opt.MapFrom(src => (ZoneUrgencyLevel)src.UrgencyLevel));

            CreateMap<EvacuationZoneEntity, EvacuationZoneResponse>()
                .ForMember(dest => dest.UrgencyLevel, opt => opt.MapFrom(src => (int)src.UrgencyLevel));
            #endregion
        }
    }
}
