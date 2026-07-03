using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class PlanCategoryConfiguration : IEntityTypeConfiguration<PlanCategory>
    {
        public void Configure(EntityTypeBuilder<PlanCategory> builder)
        {
            //Name And Schema
            builder.ToTable(nameof(PlanCategory), nameof(Plan));

            //Navigations
            builder.HasMany(t => t.Plans).WithOne(t => t.PlanCategory).HasForeignKey(t => t.PlanCategoryId);

            //Filters
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
