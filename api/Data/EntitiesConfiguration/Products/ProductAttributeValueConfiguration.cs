using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    //جدول واسط
    public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.ToTable(nameof(ProductAttributeValue), Schema.Marketplace);

            builder.HasKey(t => new { t.ProductId, t.AttributeId });

            builder.Property(av => av.Value).HasMaxLength(250).IsRequired();

            builder.HasOne(t => t.Product)
                .WithMany(t => t.AttributeValues)
                .HasForeignKey(t => t.ProductId);

            builder.HasOne(t => t.Attribute)
                .WithMany(t => t.AttributeValues)
                .HasForeignKey(t => t.AttributeId);

        }
    }
}
