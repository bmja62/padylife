using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Orders.Commands
{
    public class UpdateOrderStatusCommand(long orderId, OrderStatus newStatus) : ICommand<ServiceResult>
    {
        public long OrderId { get; set; } = orderId;
        public OrderStatus NewStatus { get; set; } = newStatus;
    }

    public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, ServiceResult>
    {
        private readonly IRepository<Order> _orderRepository;

        public UpdateOrderStatusCommandHandler(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ServiceResult> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // دریافت سفارش از دیتابیس
            var order = await _orderRepository.Table
                .Where(o => o.Id == request.OrderId)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
                return ServiceResult.NotFound("سفارش یافت نشد");

            // تغییر وضعیت سفارش
            order.Status = request.NewStatus;

            // ذخیره تغییرات
            await _orderRepository.UpdateAsync(order, cancellationToken);

            return ServiceResult.Ok("وضعیت سفارش با موفقیت تغییر کرد");
        }
    }
}
