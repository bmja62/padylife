namespace Application.Inventories.DTOs
{
    public class AdjustStockDTO
    {
        public long ProductId { get; set; }
        public long? VariantId { get; set; }
        public long WarehouseId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }

    public class TransferStockDTO : AdjustStockDTO
    {
        public long ToWarehouseId { get; set; }
        public long FromWarehouseId { get; set; }
    }

    public class ReserveStockDTO
    {
        public long ProductId { get; set; }
        public long? VariantId { get; set; }
        public long WarehouseId { get; set; }
        public int Quantity { get; set; }
        public string ReferenceId { get; set; }
    }

    public class ReleaseStockDTO : ReserveStockDTO
    {
    }

}