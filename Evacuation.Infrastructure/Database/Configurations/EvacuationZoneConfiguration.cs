using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evacuation.Infrastructure.Database.Configurations
{
    public class EvacuationZoneConfiguration : IEntityTypeConfiguration<EvacuationZoneEntity>
    {
        public void Configure(EntityTypeBuilder<EvacuationZoneEntity> builder)
        {
            builder.HasKey(e => e.ZoneId);

            builder.ToTable("EvacuationZones");
        }
    }
}
