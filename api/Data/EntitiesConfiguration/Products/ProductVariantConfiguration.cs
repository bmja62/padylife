using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable(nameof(ProductVariant), Schema.Marketplace);

            builder.Property(pv => pv.SKU).HasMaxLength(50).IsRequired();
            builder.Property(pv => pv.Price).HasColumnType("decimal(18,2)");
            builder.Property(v => v.Barcode).HasMaxLength(50);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.HasOne(t => t.Product)
                .WithMany(t => t.Variants)
                .HasForeignKey(t => t.ProductId);

            builder.HasMany(t => t.AttributeValues)
                .WithOne(t => t.Variant)
                .HasForeignKey(t => t.VariantId);

            builder.HasMany(v => v.Inventories)
               .WithOne(i => i.Variant)
               .HasForeignKey(i => i.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pv => pv.SKU).IsUnique();
            builder.HasIndex(v => v.Barcode).IsUnique();

            builder.HasQueryFilter(pv => !pv.IsDeleted);
        }
    }
}
