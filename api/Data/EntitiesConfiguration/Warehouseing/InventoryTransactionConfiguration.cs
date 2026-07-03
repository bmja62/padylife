using Common.Shemas;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Warehouseing
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable(nameof(InventoryTransaction), Schema.Inventory);

            builder.Property(t => t.ReferenceId).HasMaxLength(50);
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.PerformedBy).HasMaxLength(100);

            builder.HasOne(t => t.SourceWarehouse)
                .WithMany(w => w.OutgoingTransactions)
                .HasForeignKey(t => t.SourceWarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.DestinationWarehouse)
                .WithMany(w => w.IncomingTransactions)
                .HasForeignKey(t => t.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
