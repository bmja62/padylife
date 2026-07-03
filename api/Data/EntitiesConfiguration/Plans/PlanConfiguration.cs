using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            //Name and Schema
            builder.ToTable(nameof(Plan), nameof(Plan));

            //Navigations 
            builder.HasMany(t => t.PlanQuestions).WithOne(t => t.Plan).HasForeignKey(t => t.PlanId);
            builder.HasOne(t => t.PlanCategory).WithMany(t => t.Plans).HasForeignKey(t => t.PlanCategoryId);
            builder.HasOne(t => t.OwnerUser).WithMany(t => t.Plans).HasForeignKey(t => t.OwnerUserId);



            builder.HasMany(t => t.NextPlans)
       .WithOne(t => t.SourcePlan)
       .HasForeignKey(t => t.SourcePlanId)
       .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.PreviousPlans)
                   .WithOne(t => t.TargetPlan)
                   .HasForeignKey(t => t.TargetPlanId)
                   .OnDelete(DeleteBehavior.Restrict);
            //Filters
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
