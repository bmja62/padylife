using Entities.Baskets;

namespace Application.Baskets.DTOs
{
    public class UpdateBasketItemQuantityCommandDTO
    {
        public long ItemId { get; set; }
        public BasketItemType ItemType { get; set; }
        public int NewQuantity { get; set; }
    }
}
