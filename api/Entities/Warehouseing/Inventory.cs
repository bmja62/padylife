using Entities.Common;
using Entities.Products;

namespace Entities.Warehouseing
{
    public class Inventory : BaseEntity<long>
    {
        public long WarehouseId { get; set; }
        public long? ZoneId { get; set; }
        public long ProductId { get; set; }
        public long? VariantId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int MinimumStock { get; set; }
        public int ReorderPoint { get; set; }
        public DateTime? LastStockUpdate { get; set; }


        // Updated AvailableQuantity with private setter
        public int AvailableQuantity { get; private set; }

        // Updated NeedsReorder (if you want it database-computed)
        public bool NeedsReorder() => AvailableQuantity <= ReorderPoint;

        // Navigation properties
        public Warehouse Warehouse { get; set; }
        public WarehouseZone Zone { get; set; }
        public Product Product { get; set; }
        public ProductVariant Variant { get; set; }
        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();

        // Business methods
        public void IncreaseStock(int quantity, string reason, InventoryTransactionType type)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            Quantity += quantity;
            LastStockUpdate = DateTime.UtcNow;

            Transactions.Add(new InventoryTransaction
            {
                InventoryId = Id,
                Type = type,
                Quantity = quantity,
                Description = reason,
                TransactionDate = DateTime.UtcNow
            });
        }

        public void DecreaseStock(int quantity, string reason, InventoryTransactionType type)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (AvailableQuantity < quantity)
                throw new InvalidOperationException("Not enough available stock");

            Quantity -= quantity;
            LastStockUpdate = DateTime.UtcNow;

            Transactions.Add(new InventoryTransaction
            {
                InventoryId = Id,
                Type = type,
                Quantity = -quantity,
                Description = reason,
                TransactionDate = DateTime.UtcNow
            });
        }

        public void ReserveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (AvailableQuantity < quantity)
                throw new InvalidOperationException("Not enough available stock to reserve");

            ReservedQuantity += quantity;
        }

        public void ReleaseReservedStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (ReservedQuantity < quantity)
                throw new InvalidOperationException("Not enough reserved stock to release");

            ReservedQuantity -= quantity;
        }

        public void TransferTo(Inventory targetInventory, int quantity, string reason)
        {
            if (targetInventory == null)
                throw new ArgumentNullException(nameof(targetInventory));

            if (targetInventory.ProductId != ProductId || targetInventory.VariantId != VariantId)
                throw new InvalidOperationException("Cannot transfer between different products/variants");

            DecreaseStock(quantity, reason, InventoryTransactionType.TransferOut);
            targetInventory.IncreaseStock(quantity, reason, InventoryTransactionType.TransferIn);
        }

        public void AdjustStock(int quantity, string reason, InventoryTransactionType type)
        {
            if (quantity == 0)
                return;

            if (quantity > 0)
                IncreaseStock(quantity, reason, type);
            else
                DecreaseStock(Math.Abs(quantity), reason, type);
        }
    }
}