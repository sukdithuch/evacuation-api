using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Domain.Entities
{
    public class VehicleEntity : BaseEntity, IBaseEntity
    {
        public int VehicleId { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public bool IsAvailable { get; set; }
        //public VehicleStatus Status { get; set; }
    }

    public enum VehicleStatus
    {
        Available,
        Busy,
        Maintenance
    }
}
