using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Domain.Entities
{
    public class BaseEntity : IBaseEntity
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool Active { get; set; }
    }
}
