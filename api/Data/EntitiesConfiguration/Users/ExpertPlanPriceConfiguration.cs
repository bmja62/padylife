using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class ExpertPlanPriceConfiguration : IEntityTypeConfiguration<ExpertPlanPrice>
    {
        public void Configure(EntityTypeBuilder<ExpertPlanPrice> builder)
        {
            builder.ToTable(nameof(ExpertPlanPrice), nameof(User));
            builder.HasOne(t => t.Expert).WithMany(t => t.PlanPrices).HasForeignKey(t => t.ExpertId);
            builder.HasOne(t => t.Plan).WithMany(t => t.PlanPrices).HasForeignKey(t => t.PlanId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
