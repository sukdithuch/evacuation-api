namespace Evacuation.Core.DTOs.Responses
{
    public class EvacuationPlanResponse
    {
        public int PlanId { get; set; }
        public int ZoneId { get; set; }
        public int VehicleId { get; set; }
        public int EstimatedArrivalMinutes { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
