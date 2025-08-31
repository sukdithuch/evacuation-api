using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.ToTable("Vehicles");
        }
    }
}
