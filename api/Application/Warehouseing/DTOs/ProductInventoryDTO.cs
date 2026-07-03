namespace Application.Warehouseing.DTOs
{

    namespace Application.Inventory.DTOs
    {
        public class ProductInventoryDTO
        {
            public long WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public long? ZoneId { get; set; }
            public string ZoneName { get; set; }
            public int Quantity { get; set; }
            public int ReservedQuantity { get; set; }
            public int AvailableQuantity { get; set; }
            public int MinimumStock { get; set; }
            public int ReorderPoint { get; set; }
            public DateTime? LastStockUpdate { get; internal set; }
            public string ProductName { get; internal set; }
            public string VariantName { get; internal set; }
        }

        public class WarehouseingAdjustStockDTO
        {
            public long ProductId { get; set; }
            public long? VariantId { get; set; }
            public long WarehouseId { get; set; }
            public int Quantity { get; set; }
            public string Reason { get; set; }
        }

        public class WarehouseingTransferStockDTO : WarehouseingAdjustStockDTO
        {
            public long ToWarehouseId { get; set; }
        }

        public class WarehouseingReserveStockDTO
        {
            public long ProductId { get; set; }
            public long? VariantId { get; set; }
            public long WarehouseId { get; set; }
            public int Quantity { get; set; }
            public string ReferenceId { get; set; } // مثلاً شماره سفارش
        }

        public class WarehouseingReleaseStockDTO : WarehouseingReserveStockDTO
        {
        }

        public class InventoryTransactionDTO
        {
            public DateTime TransactionDate { get; set; }
            public string Type { get; set; }
            public int Quantity { get; set; }
            public string Description { get; set; }
            public string ReferenceId { get; set; }
            public string WarehouseName { get; set; }
        }

        public class LowStockItemDTO
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public long? VariantId { get; set; }
            public string VariantName { get; set; }
            public int AvailableQuantity { get; set; }
            public int ReorderPoint { get; set; }
            public string WarehouseName { get; set; }
        }
    }
}
