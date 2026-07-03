using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class UserDiseaseConfiguration : IEntityTypeConfiguration<UserDisease>
    {
        public void Configure(EntityTypeBuilder<UserDisease> builder)
        {
            builder.HasOne(t => t.User).WithMany(t => t.UserDiseases).HasForeignKey(t => t.UserId);
            builder.HasQueryFilter(z => !z.IsDeleted);
        }
    }
}
