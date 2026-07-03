using Entities.Baskets;

namespace Application.Baskets.DTOs
{
    public class BasketDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public List<BasketItemDTO> Items { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalPrice { get; set; }
        public BasketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class BasketItemDTO
    {
        public long ObjectId { get; set; }
        public BasketItemType ItemType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public long Id { get; internal set; }
    }

    public class AddBasketItemDTO
    {
        public long ObjectId { get; set; }
        public BasketItemType ItemType { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketHistoryDTO
    {
        public long Id { get; set; }
        public long BasketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class GetBasketHistoryQueryDTO
    {
        public long UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int Count { get; set; } = 20;
    }
}
