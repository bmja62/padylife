using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable(nameof(ProductCategory), Schema.Marketplace);

            builder.Property(pc => pc.Name).HasMaxLength(100).IsRequired();
            builder.Property(pc => pc.Description).HasMaxLength(500);


            builder.HasMany(t => t.ChildCategories)
                .WithOne(t => t.ParentCategory)
                .HasForeignKey(t => t.ParentCategoryId);

            builder.HasMany(t => t.Products)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId);

            builder.HasMany(t => t.Attributes)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
