namespace Evacuation.Core.DTOs.Requests
{
    public class EvacuationZoneRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}
