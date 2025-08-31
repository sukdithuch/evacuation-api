namespace Evacuation.Domain.Entities
{
    public class EvacuationPlanEntity : BaseEntity, IBaseEntity
    {
        public int PlanId { get; set; }
        public int ZoneId { get; set; }
        public EvacuationZoneEntity Zone { get; set; }
        public int VehicleId { get; set; }
        public VehicleEntity Vehicle { get; set; }
        public int EstimatedArrivalMinutes { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
