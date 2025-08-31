using Evacuation.Domain.Entities;

namespace Evacuation.Core.DTOs.Responses
{
    public class EvacuationZoneResponse : BaseEntity
    {
        public int ZoneId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}
