using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Domain.Entities
{
    public interface IBaseEntity
    {
        string CreatedAt { get; set; }
        string UpdatedAt { get; set; }
        bool Active { get; set; }
    }
}
