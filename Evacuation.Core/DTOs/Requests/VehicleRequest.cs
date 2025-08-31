namespace Evacuation.Core.DTOs.Requests
{
    public class VehicleRequest
    {
        public int Capacity { get; set; }
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }
}
