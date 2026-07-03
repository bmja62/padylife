using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product), nameof(Schema.Marketplace));

            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.HasMany(t => t.AttributeValues)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId);

            builder.HasMany(t => t.Variants)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId);

            builder.HasOne(t => t.Category)
                .WithMany(t => t.Products)
                .HasForeignKey(t => t.CategoryId);

            builder.HasOne(t => t.CreatedByUser)
                .WithMany(t => t.Products)
                .HasForeignKey(t => t.CreatedByUserId);

            // New configurations
            builder.Property(p => p.Barcode).HasMaxLength(50);
            builder.HasIndex(p => p.Barcode).IsUnique();

            // Navigation configurations
            builder.HasMany(p => p.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
