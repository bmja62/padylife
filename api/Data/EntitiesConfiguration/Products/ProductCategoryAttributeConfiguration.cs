using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    //جدول واسط
    public class ProductCategoryAttributeConfiguration : IEntityTypeConfiguration<ProductCategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductCategoryAttribute> builder)
        {
            builder.ToTable(nameof(ProductCategoryAttribute), Schema.Marketplace);

            builder.HasKey(t => new { t.CategoryId, t.AttributeId });

            builder.HasOne(t => t.Category)
                .WithMany(t => t.Attributes)
                .HasForeignKey(t => t.CategoryId);

            builder.HasOne(t => t.Attribute)
                .WithMany(t => t.CategoryAttributes)
                .HasForeignKey(t => t.AttributeId);
        }
    }
}
