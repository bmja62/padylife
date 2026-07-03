using Entities.Orders;

namespace Application.Orders.DTOs
{
    public class UpdateOrderStatusDTO
    {
        public OrderStatus NewStatus { get; set; }
    }
}
