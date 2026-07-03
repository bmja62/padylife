using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable(nameof(ProductAttribute), Schema.Marketplace);


            builder.Property(pa => pa.Name).HasMaxLength(100).IsRequired();
            builder.Property(pa => pa.Description).HasMaxLength(500);

            builder.HasMany(t => t.CategoryAttributes)
                .WithOne(t => t.Attribute)
                .HasForeignKey(t => t.AttributeId);

            builder.HasMany(t => t.AttributeValues)
                .WithOne(t => t.Attribute)
                .HasForeignKey(t => t.AttributeId);

            builder.HasMany(pa => pa.VariantAttributeValues)
                .WithOne(vav => vav.Attribute)
                .HasForeignKey(vav => vav.AttributeId);


            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
