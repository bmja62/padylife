using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class UserPlanCompanionConfiguration : IEntityTypeConfiguration<UserPlanCompanion>
    {
        public void Configure(EntityTypeBuilder<UserPlanCompanion> builder)
        {
            builder.ToTable(nameof(UserPlanCompanion), nameof(User));

            builder.HasOne(t => t.CompanionUser).WithMany(t => t.Companions).HasForeignKey(t => t.CompanionUserId);
            builder.HasOne(t => t.UserPlan).WithMany(t => t.Companions).HasForeignKey(t => t.UserPlanId);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
