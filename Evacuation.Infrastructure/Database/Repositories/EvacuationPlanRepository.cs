using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database.Repositories
{
    public class EvacuationPlanRepository : GenericRepository<EvacuationPlanEntity>, IEvacuationPlanRepository
    {
        //private readonly DataContext _context;

        public EvacuationPlanRepository(DataContext context) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //public async Task<List<EvacuationPlanEntity>> FindByZoneAndVehicleAsync(int zoneId, int vehicleId)
        //{
        //    return await _context.Set<T>().Where(x => x.Active).ToListAsync();
        //}
    }
}
