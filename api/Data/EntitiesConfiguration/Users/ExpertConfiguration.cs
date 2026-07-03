using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class ExpertConfiguration : IEntityTypeConfiguration<Expert>
    {
        public void Configure(EntityTypeBuilder<Expert> builder)
        {
            builder.HasMany(t => t.UserPlanExperts).WithOne(t => t.Expert).HasForeignKey(t => t.ExpertId);
            builder.HasMany(t => t.PlanPrices).WithOne(t => t.Expert).HasForeignKey(t => t.ExpertId);
        }
    }
}
