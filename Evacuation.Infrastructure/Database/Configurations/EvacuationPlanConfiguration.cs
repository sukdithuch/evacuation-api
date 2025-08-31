using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evacuation.Infrastructure.Database.Configurations
{
    public class EvacuationPlanConfiguration : IEntityTypeConfiguration<EvacuationPlanEntity>
    {
        public void Configure(EntityTypeBuilder<EvacuationPlanEntity> builder)
        {
            builder.HasKey(e => e.PlanId);

            builder.ToTable("EvacuationPlans");
        }
    }
}
