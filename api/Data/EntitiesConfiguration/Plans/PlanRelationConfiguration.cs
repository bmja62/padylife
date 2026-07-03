using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class PlanRelationConfiguration : IEntityTypeConfiguration<PlanRelation>
    {
        public void Configure(EntityTypeBuilder<PlanRelation> builder)
        {
            builder.ToTable(nameof(PlanRelation), nameof(Plan));

            builder.HasKey(t => new { t.SourcePlanId, t.TargetPlanId });

            builder.Property(t => t.Order)
                   .IsRequired();
        }
    }

}
