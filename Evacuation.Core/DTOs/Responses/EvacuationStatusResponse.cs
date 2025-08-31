namespace Evacuation.Core.DTOs.Responses
{
    public class EvacuationStatusResponse
    {
        public int ZoneId { get; set; }
        public int TotalEvacuated { get; set; }
        public int RemainingPeople { get; set; }
        public int? LastVehicleUsedId { get; set; }
    }
}
