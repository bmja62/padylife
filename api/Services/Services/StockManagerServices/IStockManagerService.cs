using Common;
using Data.Contracts;
using Entities.Orders;
using Entities.Products;
using Entities.Warehouseing;
using Services.Services.Warehousing.ProductStockServices;
using Services.Services.Warehousing.WarehouseServices;

namespace Services.Services.StockManagerServices
{
    public interface IStockManagerService
    {
        Task<int> GetAvailableStockAsync(Product product, long? warehouseId = null);
        Task<int> GetAvailableStockAsync(ProductVariant variant, long? warehouseId = null);
        Task<bool> HasStockAsync(Product product, int quantity, long? warehouseId = null);
        Task<bool> HasStockAsync(ProductVariant variant, int quantity, long? warehouseId = null);
        Task DecreaseStockAsync(Product product, int quantity, long warehouseId, string reason);
        Task DecreaseStockAsync(ProductVariant variant, int quantity, long warehouseId, string reason);
        Task IncreaseStockAsync(Product product, int quantity, long warehouseId, string reason);
        Task IncreaseStockAsync(ProductVariant variant, int quantity, long warehouseId, string reason);
        Task ReturnStockAsync(IReadOnlyCollection<OrderItem> items, long warehouseId,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            CancellationToken cancellationToken);
    }

    public class StockManagerService : IStockManagerService, IScopedDependency
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IProductStockService _productStockService;

        public StockManagerService(
            IWarehouseService warehouseService,
            IProductStockService productStockService)
        {
            _warehouseService = warehouseService;
            _productStockService = productStockService;
        }

        public async Task<int> GetAvailableStockAsync(Product product, long? warehouseId = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Variants != null && product.Variants.Any())
            {
                return await _warehouseService.GetAvailableStockAsync(product.Id, null, warehouseId);
            }

            return await _warehouseService.GetAvailableStockAsync(product.Id, null, warehouseId);
        }

        public async Task<int> GetAvailableStockAsync(ProductVariant variant, long? warehouseId = null)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            return await _warehouseService.GetAvailableStockAsync(variant.ProductId, variant.Id, warehouseId);
        }

        public async Task<bool> HasStockAsync(Product product, int quantity, long? warehouseId = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var availableStock = await GetAvailableStockAsync(product, warehouseId);
            return availableStock >= quantity;
        }

        public async Task<bool> HasStockAsync(ProductVariant variant, int quantity, long? warehouseId = null)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var availableStock = await GetAvailableStockAsync(variant, warehouseId);
            return availableStock >= quantity;
        }

        public async Task DecreaseStockAsync(Product product, int quantity, long warehouseId, string reason)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Variants != null && product.Variants.Any())
            {
                throw new InvalidOperationException("Please decrease stock from specific variants.");
            }

            await _warehouseService.DecreaseStockAsync(
                product.Id,
                null,
                warehouseId,
                quantity,
                InventoryTransactionType.Sale,
                reason);
        }

        public async Task DecreaseStockAsync(ProductVariant variant, int quantity, long warehouseId, string reason)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            await _warehouseService.DecreaseStockAsync(
                variant.ProductId,
                variant.Id,
                warehouseId,
                quantity,
                InventoryTransactionType.Sale,
                reason);
        }

        public async Task IncreaseStockAsync(Product product, int quantity, long warehouseId, string reason)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            if (product.Variants != null && product.Variants.Any())
            {
                throw new InvalidOperationException("Please increase stock for specific variants.");
            }

            await _warehouseService.IncreaseStockAsync(
                product.Id,
                null,
                warehouseId,
                quantity,
                InventoryTransactionType.Purchase,
                reason);
        }

        public async Task IncreaseStockAsync(ProductVariant variant, int quantity, long warehouseId, string reason)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            await _warehouseService.IncreaseStockAsync(
                variant.ProductId,
                variant.Id,
                warehouseId,
                quantity,
                InventoryTransactionType.Purchase,
                reason);
        }

        public async Task ReturnStockAsync(
            IReadOnlyCollection<OrderItem> items,
            long warehouseId,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                if (item.ItemType == OrderItemType.Product)
                {
                    var product = await productRepository.GetByIdAsync(cancellationToken, item.ObjectId);
                    if (product != null)
                    {
                        await this.IncreaseStockAsync(
                            product,
                            item.Quantity,
                            warehouseId,
                            $"Return from order {item.OrderId}");
                    }
                }
                else if (item.ItemType == OrderItemType.Variant)
                {
                    var variant = await productVariantRepository.GetByIdAsync(cancellationToken, item.ObjectId);
                    if (variant != null)
                    {
                        await this.IncreaseStockAsync(
                            variant,
                            item.Quantity,
                            warehouseId,
                            $"Return from order {item.OrderId}");
                    }
                }
            }
        }
    }
}