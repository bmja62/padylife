using Entities.Baskets;

namespace Application.Baskets.DTOs
{
    public class RemoveItemFromBasketCommandDTO
    {
        public long ItemId { get; set; }
        public BasketItemType BasketItemType { get; set; }
    }
}
