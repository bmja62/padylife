using Data.Contracts;
using Entities.Common.Events;
using Entities.Orders;
using Entities.Products;
using Microsoft.Extensions.Logging;
using Services.Services.StockManagerServices;
using Services.Services.Warehousing.WarehouseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders.Events
{
    public class OrderPaymentFailedEvent(long userId, ICollection<OrderItem> orderItems) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public ICollection<OrderItem> OrderItems { get; } = orderItems;
    }
    public class ReplenishStockOnPaymentFailureHandler
    : IDomainEventAction<OrderPaymentFailedEvent>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly IStockManagerService _stockManagerService;
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<ReplenishStockOnPaymentFailureHandler> _logger;

        public ReplenishStockOnPaymentFailureHandler(
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            IStockManagerService stockManagerService,
            IWarehouseService warehouseService,
            ILogger<ReplenishStockOnPaymentFailureHandler> logger)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _stockManagerService = stockManagerService;
            _warehouseService = warehouseService;
            _logger = logger;
        }

        public async Task ExecuteAsync(OrderPaymentFailedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                var defaultWarehouse = await _warehouseService.GetDefaultWarehouseAsync();
                if (defaultWarehouse == null)
                    throw new InvalidOperationException("No default warehouse configured");

                // For each order item, revert stock
                foreach (var orderItem in @event.OrderItems)
                {
                    if (orderItem.ItemType == OrderItemType.Product)
                    {
                        var product = await _productRepository.GetByIdAsync(CancellationToken.None, orderItem.ObjectId);
                        if (product == null) continue;

                        // Replenish stock
                        await _stockManagerService.IncreaseStockAsync(
                            product,
                            orderItem.Quantity,
                            defaultWarehouse.Id,
                            $"Reverting order #{orderItem.OrderId} due to payment failure");
                    }
                    else if (orderItem.ItemType == OrderItemType.Variant)
                    {
                        var productVariant = await _productVariantRepository.GetByIdAsync(CancellationToken.None, orderItem.ObjectId);
                        if (productVariant == null) continue;

                        // Replenish stock for variants
                        await _stockManagerService.IncreaseStockAsync(
                            productVariant,
                            orderItem.Quantity,
                            defaultWarehouse.Id,
                            $"Reverting order #{orderItem.OrderId} due to payment failure");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while replenishing stock for failed payment for User {UserId}", @event.UserId);
                throw;
            }
        }
    }

}
