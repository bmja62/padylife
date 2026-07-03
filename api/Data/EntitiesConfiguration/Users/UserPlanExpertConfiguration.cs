using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class UserPlanExpertConfiguration : IEntityTypeConfiguration<UserPlanExpert>
    {
        public void Configure(EntityTypeBuilder<UserPlanExpert> builder)
        {
            builder.ToTable(nameof(UserPlanExpert), nameof(User));
            builder.HasOne(t => t.Expert).WithMany(t => t.UserPlanExperts).HasForeignKey(t => t.ExpertId);
            builder.HasOne(t => t.UserPlan).WithMany(t => t.Experts).HasForeignKey(t => t.UserPlanId);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
