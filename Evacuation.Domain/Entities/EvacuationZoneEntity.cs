

namespace Evacuation.Domain.Entities
{
    public class EvacuationZoneEntity : BaseEntity, IBaseEntity
    {
        public int ZoneId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int NumberOfPeople { get; set; }
        public ZoneUrgencyLevel UrgencyLevel { get; set; }
        public int TotalEvacuated { get; set; }
        public int RemainingPeople { get; set; }
        public int? LastVehicleUsedId { get; set; }
    }

    public enum ZoneUrgencyLevel
    {
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        VeryHigh = 5
    }
}
