using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class UserPlanConfiguration : IEntityTypeConfiguration<UserPlan>
    {
        public void Configure(EntityTypeBuilder<UserPlan> builder)
        {
            //Name and Schema
            builder.ToTable(nameof(UserPlan), nameof(Plan));

            //Navigation
            builder.HasOne(t => t.User).WithMany(t => t.UserPlans).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Plan).WithMany(t => t.UserPlans).HasForeignKey(t => t.PlanId);
            builder.HasMany(t => t.Answers).WithOne(t => t.UserPlan).HasForeignKey(t => t.UserPlanId);

            //Filters
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
