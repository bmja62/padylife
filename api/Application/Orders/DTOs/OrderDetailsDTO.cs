namespace Application.Orders.DTOs
{
    public class OrderDetailsDTO
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public long UserId { get; internal set; }
        public UserOrderInfoDTO UserInfo { get; internal set; }
        public List<OrderItemDTO> Items { get; set; }
        public string PaymentStatus { get; internal set; }
        public string Address { get; internal set; }
    }

    public class OrderItemDTO
    {
        public long ObjectId { get; set; }
        public string Title { get; set; }  // ترکیب نام محصول و SKU برای آیتم‌های متغیر
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemType { get; set; } // Product یا Variant
    }
}
