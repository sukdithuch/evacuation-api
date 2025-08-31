namespace Evacuation.Domain.Entities
{
    public class EvacuationLogEntity : BaseEntity, IBaseEntity
    {
        public int LogId { get; set; }
        public int ZoneId { get; set; }
        public EvacuationZoneEntity Zone { get; set; }
        public int VehicleId { get; set; }
        public VehicleEntity Vehicle { get; set; }
        public int EstimatedArrivalMinutes { get; set; }
        public int NumberOfPeople { get; set; }
        public int? EvacuatedPeople { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
