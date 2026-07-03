using Entities.Discounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Discounts
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            // کلید اصلی
            builder.HasKey(d => d.Id);

            // ویژگی‌ها
            builder.Property(d => d.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(d => d.DiscountAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(d => d.DiscountPercentage)
                   .IsRequired()
                   .HasColumnType("decimal(5,2)");

            builder.Property(d => d.StartDate)
                   .HasColumnType("timestamp without time zone");

            builder.Property(d => d.EndDate)
                   .HasColumnType("timestamp without time zone");

            builder.Property(d => d.IsSpecial)
                   .IsRequired();

            builder.HasQueryFilter(t => !t.IsDeleted);

            // اطمینان از منحصربه‌فرد بودن کد تخفیف
            builder.HasIndex(d => d.Code)
                   .IsUnique();
        }
    }
}
