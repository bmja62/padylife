using Entities.Common;

namespace Entities.Warehouseing
{
    public class InventoryTransaction : BaseEntity<long>
    {
        public long InventoryId { get; set; }
        public InventoryTransactionType Type { get; set; }
        public int Quantity { get; set; } // مثبت برای افزایش، منفی برای کاهش
        public string ReferenceId { get; set; } // شماره فاکتور/سند مرتبط
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public long? SourceWarehouseId { get; set; }
        public long? DestinationWarehouseId { get; set; }
        public string PerformedBy { get; set; } // کاربر انجام دهنده

        // Navigation properties
        public Inventory Inventory { get; set; }
        public Warehouse SourceWarehouse { get; set; }
        public Warehouse DestinationWarehouse { get; set; }

        // Business methods
        public string GetTransactionSummary()
        {
            var action = Quantity > 0 ? "Increase" : "Decrease";
            return $"{Type} {action} of {Math.Abs(Quantity)} units on {TransactionDate:yyyy-MM-dd}";
        }
    }
}
