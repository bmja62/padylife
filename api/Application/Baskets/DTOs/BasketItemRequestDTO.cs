using Entities.Baskets;

namespace Application.Baskets.DTOs
{
    public class BasketItemRequestDTO
    {
        public long ObjectId { get; set; }
        public BasketItemType ItemType { get; set; }
        public int Quantity { get; set; }
    }
}
