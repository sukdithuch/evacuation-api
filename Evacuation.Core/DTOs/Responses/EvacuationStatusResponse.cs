using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
