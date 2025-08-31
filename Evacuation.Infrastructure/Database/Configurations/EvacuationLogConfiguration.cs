using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evacuation.Infrastructure.Database.Configurations
{
    public class EvacuationLogConfiguration : IEntityTypeConfiguration<EvacuationLogEntity>
    {
        public void Configure(EntityTypeBuilder<EvacuationLogEntity> builder)
        {
            builder.HasKey(e => e.LogId);

            builder.Ignore(l => l.UpdatedAt);

            builder.Ignore(l => l.Active);

            builder.ToTable("EvacuationLogs");
        }
    }
}
