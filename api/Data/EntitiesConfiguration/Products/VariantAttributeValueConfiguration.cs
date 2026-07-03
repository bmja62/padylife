using Common.Shemas;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Products
{
    //جدول واسط
    public class VariantAttributeValueConfiguration : IEntityTypeConfiguration<VariantAttributeValue>
    {
        public void Configure(EntityTypeBuilder<VariantAttributeValue> builder)
        {
            builder.ToTable(nameof(VariantAttributeValue), Schema.Marketplace);

            builder.HasKey(t => new { t.VariantId, t.AttributeId });

            builder.HasOne(t => t.Variant)
                .WithMany(t => t.AttributeValues)
                .HasForeignKey(t => t.VariantId);

            builder.HasOne(t => t.Attribute)
                .WithMany(t => t.VariantAttributeValues)
                .HasForeignKey(t => t.AttributeId);

        }
    }
}
