namespace Application.Orders.DTOs
{
    public class OrderListItemDTO
    {
        public long OrderId { get; set; }
        public string OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public long UserId { get; internal set; }
        public UserOrderInfoDTO UserInfo { get; internal set; }
        public List<OrderItemSummaryDTO> Items { get; set; }
        public string Address { get; internal set; }
    }

    public class OrderItemSummaryDTO
    {
        public long ObjectId { get; set; }
        public string Title { get; set; }  // ترکیب نام محصول و SKU برای آیتم‌های متغیر
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemType { get; set; } // Product یا Variant
    }
    public class UserOrderInfoDTO
    {
        public long Id { get; internal set; }
        public string FullName { get; internal set; }
        public string UserName { get; internal set; }
        public string Email { get; internal set; }
        public string PhoneNumber { get; internal set; }
    }
}
