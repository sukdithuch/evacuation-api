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
    public class VehicleConfiguration : IEntityTypeConfiguration<VehicleEntity>
    {
        public void Configure(EntityTypeBuilder<VehicleEntity> builder)
        {
            builder
                .HasKey(e => e.VehicleId);

            builder
                .Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20);

            //builder
            //    .Property(e => e.Status)
            //    .IsRequired()
            //    .HasConversion<string>()
            //    .HasMaxLength(20);

            //builder
            //    .Property(e => e.CreatedAt)
            //    .IsRequired()
            //    .HasMaxLength(20);

            //builder
            //    .Property(e => e.UpdatedAt)
            //    .IsRequired()
            //    .HasMaxLength(20);

            builder.ToTable("Vehicles");
        }
    }
}
