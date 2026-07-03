using Common.Shemas;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Warehouseing
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable(nameof(Inventory), Schema.Inventory);
            builder.HasIndex(i => new { i.WarehouseId, i.ZoneId, i.ProductId, i.VariantId }).IsUnique();

            builder.HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Variant)
                .WithMany(v => v.Inventories)
                .HasForeignKey(i => i.VariantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.Transactions)
                .WithOne(t => t.Inventory)
                .HasForeignKey(t => t.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure computed columns
            builder.Property(i => i.AvailableQuantity)
                .HasComputedColumnSql("\"Quantity\" - \"ReservedQuantity\"", stored: true)
                .ValueGeneratedOnAddOrUpdate();


        }
    }
}
