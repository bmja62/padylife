using Common.GridResults;
using Entities.Products;

namespace Application.Products.DTOs
{
    public class GetAllProductsQueryDTO : GlobalGrid
    {
        public long? CategoryId { get; set; }
        public string SearchTerm { get; set; }
    }

    public class IncreaseOrDecreaseStockDTO
    {
        public long ObjectId { get; set; }
        public ProductType Type { get; set; }
        public int Quantity { get; set; }

    }
}