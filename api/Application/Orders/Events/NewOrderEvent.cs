using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Orders.Extentions;
using Application.Plans.Commands;
using Application.Warehouseing.Queries;
using Common.Roles;
using Data.Contracts;
using Entities.Baskets;
using Entities.Common.Events;
using Entities.Orders;
using Entities.Plans;
using Entities.Products;
using Entities.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Services.NotificationServices;
using Services.Services.StockManagerServices;
using Services.Services.Warehousing.WarehouseServices;

namespace Application.Orders.Events
{
    public class NewOrderEvent(long userId, ICollection<BasketItem> basketItems, IReadOnlyCollection<OrderItem> orderItems) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public ICollection<BasketItem> BasketItems { get; } = basketItems;
        public IReadOnlyCollection<OrderItem> OrderItems { get; } = orderItems;
    }

    /// <summary>
    /// اکشن ارسال نوتیفیکیشن داخلی برای رویداد سفارش جدید
    /// </summary>
    /// <param name="sendSystemNotification"></param>
    /// <param name="userRepository"></param>
    public class SendOrderNotificationAction(ISendSystemNotification sendSystemNotification, IRepository<User> userRepository) : IDomainEventAction<NewOrderEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IRepository<User> _userRepository = userRepository;

        public async Task ExecuteAsync(NewOrderEvent @event, CancellationToken cancellationToken)
        {
            var adminId = await _userRepository.Table
                .Where(u => u.UserRoles.Any(r => r.Role.Name == UserRoleNames.Admin))
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (adminId > 0)
            {
                await _sendSystemNotification.SendNotification(
                    adminId,
                    OrderNotificationMessage.NewOrderSubject,
                    OrderNotificationMessage.NewOrderDescription,
                    false,
                    new List<long> { @event.UserId, adminId },
                    true
                );
            }
        }
    }


    /// <summary>
    /// اکشن کاهش موجودی محصول برای رویداد سفارش جدید (نسخه سازگار با سیستم انبار)
    /// </summary>
    public class DecreaseProductStockAction : IDomainEventAction<NewOrderEvent>
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly IRepository<Basket> _basketRepository;
        private readonly IRepository<BasketItem> _basketItemRepository;
        private readonly IStockManagerService _stockManagerService;
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<DecreaseProductStockAction> _logger;

        public DecreaseProductStockAction(
            IQueryDispatcher queryDispatcher,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository,
            IStockManagerService stockManagerService,
            IWarehouseService warehouseService,
            ILogger<DecreaseProductStockAction> logger)
        {
            _queryDispatcher = queryDispatcher;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _basketRepository = basketRepository;
            _basketItemRepository = basketItemRepository;
            _stockManagerService = stockManagerService;
            _warehouseService = warehouseService;
            _logger = logger;
        }

        public async Task ExecuteAsync(NewOrderEvent @event, CancellationToken cancellationToken)
        {
            try
            {

                // دریافت انبار پیش‌فرض برای کسر موجودی
                var defaultWarehouse = await _warehouseService.GetDefaultWarehouseAsync();
                if (defaultWarehouse == null)
                    throw new InvalidOperationException("No default warehouse configured");

                // دریافت محصولات و واریانت‌های مرتبط با سفارش
                var orderProductIds = @event.BasketItems
                    .Where(t => t.ItemType == BasketItemType.Product)
                    .Select(t => t.ObjectId)
                    .ToList();

                var orderProductVariantIds = @event.BasketItems
                    .Where(t => t.ItemType == BasketItemType.Variant)
                    .Select(t => t.ObjectId)
                    .ToList();

                var productVariants = await _productVariantRepository.Table
                    .Where(t => orderProductVariantIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                var products = await _productRepository.Table
                    .Include(t => t.Variants)
                    .Where(t => orderProductIds.Contains(t.Id) ||
                               t.Variants.Any(a => orderProductVariantIds.Contains(a.Id)))
                    .ToListAsync(cancellationToken);


                foreach (var item in @event.OrderItems)
                {
                    if (item.ItemType == OrderItemType.Product)
                    {
                        var result = await _queryDispatcher.SendAsync(new GetProductInventoryQuery(item.ObjectId, null));


                        var product = products.FirstOrDefault(t => t.Id == item.ObjectId);
                        if (product == null) continue;

                        defaultWarehouse = await _warehouseService.GetWarehouseAsync(result.Data.Data.Select(a => a.WarehouseId).FirstOrDefault());

                        // کاهش موجودی در انبار
                        await _stockManagerService.DecreaseStockAsync(
                            product,
                            item.Quantity,
                            defaultWarehouse.Id,
                            $"Order #{item.OrderId}");


                        // بررسی موجودی برای سبدهای دیگر
                        await UpdateOtherBasketsForProduct(
                            product.Id,
                            item.Quantity,
                            defaultWarehouse.Id);
                    }
                    else if (item.ItemType == OrderItemType.Variant)
                    {
                        var productVariant = productVariants.FirstOrDefault(t => t.Id == item.ObjectId);
                        if (productVariant == null) continue;

                        var productId = products.Where(a => a.Variants.Any(b => b.Id == item.ObjectId)).Select(a => a.Id).FirstOrDefault();
                        var result = await _queryDispatcher.SendAsync(new GetProductInventoryQuery(productId, item.ObjectId));
                        defaultWarehouse = await _warehouseService.GetWarehouseAsync(result.Data.Data.Select(a => a.WarehouseId).FirstOrDefault());

                        // کاهش موجودی واریانت در انبار
                        await _stockManagerService.DecreaseStockAsync(
                            productVariant,
                            item.Quantity,
                            defaultWarehouse.Id,
                            $"Order #{item.OrderId}");



                        // بررسی موجودی برای سبدهای دیگر
                        await UpdateOtherBasketsForVariant(
                            productVariant.Id,
                            item.Quantity,
                            defaultWarehouse.Id);


                    }
                }


            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while processing new order event.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while decreasing product stock for User {UserId}", @event.UserId);
                throw;
            }
        }

        private async Task UpdateOtherBasketsForProduct(long productId, int orderedQuantity, long warehouseId)
        {
            // بررسی موجودی فعلی
            var availableStock = await _stockManagerService.GetAvailableStockAsync(
                await _productRepository.GetByIdAsync(CancellationToken.None, productId),
                warehouseId);

            if (availableStock <= 0)
            {
                // حذف آیتم‌های سبد دیگران اگر موجودی صفر شد
                var otherBasketItems = await _basketItemRepository.Table
                    .Where(t => t.ItemType == BasketItemType.Product && t.ObjectId == productId)
                    .ToListAsync();

                await _basketItemRepository.DeleteRangeAsync(otherBasketItems, CancellationToken.None);
            }
            else
            {
                // به‌روزرسانی مقدار در سبد دیگران اگر بیشتر از موجودی باشد
                var overQuantityItems = await _basketItemRepository.Table
                    .Where(t => t.ItemType == BasketItemType.Product &&
                               t.ObjectId == productId &&
                               t.Quantity > availableStock)
                    .ToListAsync();

                foreach (var item in overQuantityItems)
                {
                    item.Quantity = availableStock;
                }

                await _basketItemRepository.UpdateRangeAsync(overQuantityItems, CancellationToken.None);
            }
        }

        private async Task UpdateOtherBasketsForVariant(long variantId, int orderedQuantity, long warehouseId)
        {
            // بررسی موجودی فعلی
            var variant = await _productVariantRepository.GetByIdAsync(CancellationToken.None, variantId);
            var availableStock = await _stockManagerService.GetAvailableStockAsync(variant, warehouseId);

            if (availableStock <= 0)
            {
                // حذف آیتم‌های سبد دیگران اگر موجودی صفر شد
                var otherBasketItems = await _basketItemRepository.Table
                    .Where(t => t.ItemType == BasketItemType.Variant && t.ObjectId == variantId)
                    .ToListAsync();

                await _basketItemRepository.DeleteRangeAsync(otherBasketItems, CancellationToken.None);
            }
            else
            {
                // به‌روزرسانی مقدار در سبد دیگران اگر بیشتر از موجودی باشد
                var overQuantityItems = await _basketItemRepository.Table
                    .Where(t => t.ItemType == BasketItemType.Variant &&
                               t.ObjectId == variantId &&
                               t.Quantity > availableStock)
                    .ToListAsync();

                foreach (var item in overQuantityItems)
                {
                    item.Quantity = availableStock;
                }

                await _basketItemRepository.UpdateRangeAsync(overQuantityItems, CancellationToken.None);
            }
        }
    }
}
