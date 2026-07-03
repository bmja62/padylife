using Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parbad.Storage.EntityFrameworkCore.Domain;

namespace Data.EntitiesConfiguration.ParbadPayments
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            // نام جدول را می‌توانید تغییر دهید (اختیاری)
            builder.ToTable(nameof(Payment), nameof(Payment));

            //// روابط
            //builder.HasOne(p => p.User)
            //      .WithMany()
            //      .HasForeignKey(p => p.UserId);

            //builder.HasOne(p => p.Order)
            //      .WithMany()
            //      .HasForeignKey(p => p.OrderId);
        }
    }
}
