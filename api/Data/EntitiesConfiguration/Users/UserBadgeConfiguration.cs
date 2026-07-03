using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Users
{
    public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
    {
        public void Configure(EntityTypeBuilder<UserBadge> builder)
        {
            builder.ToTable(nameof(UserBadge), nameof(User));
            builder.HasKey(b => b.Id);

            builder.Property(b => b.SimilarityPercent)
                   .IsRequired();

            builder.Property(b => b.BadgeType)
                   .IsRequired();

            // روابط کاربران
            builder.HasOne(b => b.User)
                   .WithMany()
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.SimilarUser)
                   .WithMany()
                   .HasForeignKey(b => b.SimilarUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ترکیب یکتا برای جلوگیری از ثبت مجدد همان Badge
            builder.HasIndex(b => new { b.UserId, b.SimilarUserId, b.BadgeType })
                   .IsUnique();

            // محدودیت‌ها و قواعد داده‌ای پیشنهادی
            builder.Property(b => b.SimilarityPercent)
                   .HasPrecision(5, 2); // مثل 95.34
        }
    }
}
