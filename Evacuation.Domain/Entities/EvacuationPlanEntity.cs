using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public PlanStatus Status { get; set; }
    }

    public enum PlanStatus
    {
        Assigned,
        InProgress,
        Completed
    }
}
