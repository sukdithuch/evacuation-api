using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation.Infrastructure.Database.Configurations
{
    public class EvacuationZoneConfiguration : IEntityTypeConfiguration<EvacuationZoneEntity>
    {
        public void Configure(EntityTypeBuilder<EvacuationZoneEntity> builder)
        {
            builder.HasKey(e => e.ZoneId);

            //builder
            //    .Property(e => e.CreatedAt)
            //    .IsRequired()
            //    .HasMaxLength(20);

            //builder
            //    .Property(e => e.UpdatedAt)
            //    .IsRequired()
            //    .HasMaxLength(20);

            builder.ToTable("EvacuationZones");
        }
    }
}
