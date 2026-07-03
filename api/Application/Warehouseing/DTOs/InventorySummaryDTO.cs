namespace Application.Warehouseing.DTOs
{
    public class InventorySummaryDTO
    {
        public int TotalQuantity { get; internal set; }
        public int TotalReserved { get; internal set; }
        public int TotalAvailable { get; internal set; }
        public int WarehouseCount { get; internal set; }
        public int ZoneCount { get; internal set; }
    }
}
