using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<EvacuationZoneEntity> EvacuationZones { get; set; }
        public DbSet<EvacuationPlanEntity> EvacuationPlans { get; set; }
        public DbSet<EvacuationLogEntity> EvacuationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new EvacuationZoneConfiguration());
            modelBuilder.ApplyConfiguration(new EvacuationPlanConfiguration());
            modelBuilder.ApplyConfiguration(new EvacuationLogConfiguration());
        }
    }
}
