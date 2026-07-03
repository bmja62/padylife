using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Orders;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.StockManagerServices;

namespace Application.Orders.Commands
{
    public class CancelOrderCommand(long orderId, long warehouseId) : ICommand<ServiceResult>
    {
        public long OrderId { get; } = orderId;
        public long WarehouseId { get; set; } = warehouseId;
    }

    public class CancelOrderCommandHandler(
    IRepository<Order> orderRepository,
    IRepository<Product> productRepository,
    IRepository<ProductVariant> productVariantRepository,
    IStockManagerService stockManagerService
    ) : ICommandHandler<CancelOrderCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                .Table
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return ServiceResult.NotFound("سفارش یافت نشد");

            if (order.Status == OrderStatus.Cancelled)
                return ServiceResult.Fail("سفارش قبلاً کنسل شده است");

            // برگرداندن موجودی‌ها
            await stockManagerService.ReturnStockAsync(order.Items, request.WarehouseId, productRepository, productVariantRepository, cancellationToken);

            // بروزرسانی وضعیت سفارش
            order.Status = OrderStatus.Cancelled;
            await orderRepository.UpdateAsync(order, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
