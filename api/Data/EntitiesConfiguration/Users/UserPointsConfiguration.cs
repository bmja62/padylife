using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class UserPointsConfiguration : IEntityTypeConfiguration<UserPoints>
    {
        public void Configure(EntityTypeBuilder<UserPoints> builder)
        {
            builder.ToTable(nameof(UserPoints), nameof(User));

            // Primary Key
            builder.HasKey(up => up.Id);

            // Properties
            builder.Property(up => up.UserId)
                .IsRequired();

            // Relations (یک به یک با User)
            builder.HasOne(up => up.User)
                .WithOne(u => u.UserPoints)
                .HasForeignKey<UserPoints>(up => up.UserId);

            builder.HasMany(up => up.PointTransactions)
                .WithOne(pt => pt.UserPoints)
                .HasForeignKey(pt => pt.UserId)
                .HasPrincipalKey(up => up.UserId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

    public class PointTransactionConfiguration : IEntityTypeConfiguration<PointTransaction>
    {
        public void Configure(EntityTypeBuilder<PointTransaction> builder)
        {
            builder.ToTable(nameof(PointTransaction), nameof(User));

            // Primary Key
            builder.HasKey(pt => pt.Id);

            // Relations
            builder.HasOne(pt => pt.UserPoints)
                .WithMany(up => up.PointTransactions)
                .HasForeignKey(pt => pt.UserId)
                .HasPrincipalKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
