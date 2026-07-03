using Application.Warehouseing.DTOs.Application.Inventory.DTOs;

namespace Application.Warehouseing.DTOs
{
    public class GetProductInventoryDTO
    {
        public List<ProductInventoryDTO> Data { get; internal set; }
        public int TotalCount { get; internal set; }
        public InventorySummaryDTO Summery { get; internal set; }
    }
}
