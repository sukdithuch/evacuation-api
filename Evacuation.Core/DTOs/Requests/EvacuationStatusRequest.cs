

namespace Evacuation.Core.DTOs.Requests
{
    public class EvacuationStatusRequest
    {
        public int ZoneId { get; set; }
        public int VehicleId { get; set; }
        public int EvacuatedPeople { get; set; }
    }
}
