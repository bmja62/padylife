using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.FullName).HasMaxLength(100);

            builder.HasDiscriminator<string>("Discriminator")
                    .HasValue<User>("User")
                    .HasValue<Expert>("Expert");
        }
    }
}
