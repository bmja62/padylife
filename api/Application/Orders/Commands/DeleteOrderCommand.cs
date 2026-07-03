using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Orders;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.StockManagerServices;

namespace Application.Orders.Commands
{
    public class DeleteOrderCommand(long orderId, long wareHouseId) : ICommand<ServiceResult>
    {
        public long OrderId { get; } = orderId;
        public long WarehouseId { get; set; }
    }
    public class DeleteOrderCommandHandler(
    IRepository<Order> orderRepository,
    IRepository<Product> productRepository,
    IRepository<ProductVariant> productVariantRepository,
    IStockManagerService stockManagerService
    ) : ICommandHandler<DeleteOrderCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                .Table
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return ServiceResult.NotFound("سفارش یافت نشد");

            // برگرداندن موجودی محصولات
            await stockManagerService.ReturnStockAsync(order.Items, request.WarehouseId, productRepository, productVariantRepository, cancellationToken);


            // حذف سفارش
            await orderRepository.DeleteAsync(order, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
