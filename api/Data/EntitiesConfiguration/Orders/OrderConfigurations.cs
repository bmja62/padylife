using Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Orders
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order), nameof(Order));

            // کلید اصلی
            builder.HasKey(o => o.Id);

            // روابط
            builder.HasMany(o => o.Items)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);

            builder.HasOne(o => o.Discount)
                   .WithMany(o => o.Orders)
                   .HasForeignKey(o => o.DiscountId);

            builder.HasOne(o => o.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.UserId);

            builder.HasOne(o => o.Address)
              .WithMany(o => o.Orders)
              .HasForeignKey(o => o.AddressId);

            // ویژگی‌ها
            builder.Property(o => o.TotalAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Status)
                   .IsRequired();

        }
    }
}
