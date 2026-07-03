using Common;
using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Services.Services.Warehousing.WarehouseServices;

namespace Services.Services.Warehousing.ProductStockServices
{
    public interface IProductStockService
    {
        Task<int> GetTotalStockAsync(long productId, long? variantId = null);
        Task<bool> HasSufficientStockAsync(long productId, long? variantId, int requiredQuantity);
        Task ReserveStockForOrderAsync(long productId, long? variantId, int quantity);
        Task CommitStockForOrderAsync(long productId, long? variantId, int quantity);
        Task ReturnStockFromOrderAsync(long productId, long? variantId, int quantity);
        Task AdjustStockAsync(long productId, long? variantId, int adjustment, string reason);
    }

    public class ProductStockService : IProductStockService, IScopedDependency
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _variantRepository;

        public ProductStockService(
            IWarehouseService warehouseService,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> variantRepository)
        {
            _warehouseService = warehouseService;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
        }

        public async Task<int> GetTotalStockAsync(long productId, long? variantId = null)
        {
            return await _warehouseService.GetAvailableStockAsync(productId, variantId);
        }

        public async Task<bool> HasSufficientStockAsync(long productId, long? variantId, int requiredQuantity)
        {
            var availableStock = await GetTotalStockAsync(productId, variantId);
            return availableStock >= requiredQuantity;
        }

        public async Task ReserveStockForOrderAsync(long productId, long? variantId, int quantity)
        {
            // ابتدا بررسی کنیم محصول وجود دارد
            var product = await _productRepository.GetByIdAsync(CancellationToken.None, productId);
            if (product == null)
                throw new Exception("Product not found");

            if (variantId.HasValue)
            {
                var variant = await _variantRepository.GetByIdAsync(CancellationToken.None, variantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new Exception("Variant not found for this product");
            }

            // استراتژی تخصیص انبار (می‌تواند پیچیده‌تر شود)
            var warehouses = await GetWarehousesWithStockAsync(productId, variantId);

            foreach (var warehouse in warehouses)
            {
                var availableInWarehouse = await _warehouseService.GetAvailableStockAsync(
                    productId, variantId, warehouse.Id);

                var toReserve = Math.Min(availableInWarehouse, quantity);

                if (toReserve > 0)
                {
                    await _warehouseService.ReserveStockAsync(
                        productId, variantId, warehouse.Id, toReserve);

                    quantity -= toReserve;
                    if (quantity == 0) break;
                }
            }

            if (quantity > 0)
                throw new Exception($"Insufficient stock for product {productId}");
        }

        public async Task CommitStockForOrderAsync(long productId, long? variantId, int quantity)
        {
            // پیاده‌سازی مشابه ReserveStockForOrderAsync اما با کاهش موجودی واقعی
            // ...
        }

        private async Task<List<Warehouse>> GetWarehousesWithStockAsync(long productId, long? variantId)
        {
            // در اینجا می‌توانید منطق پیشرفته‌تری برای انتخاب انبارها داشته باشید
            // مثلاً بر اساس نزدیکی به مشتری یا سیاست‌های دیگر
            //return await _warehouseService.GetAllActiveWarehousesAsync();
            throw new NotImplementedException();

        }

        public async Task ReturnStockFromOrderAsync(long productId, long? variantId, int quantity)
        {
            // دریافت انبار پیش‌فرض
            var defaultWarehouse = await _warehouseService.GetDefaultWarehouseAsync();
            if (defaultWarehouse == null)
                throw new Exception("No default warehouse configured");

            // بررسی موجودی رزرو شده
            var reservedStock = await _warehouseService.GetReservedStockAsync(
                productId,
                variantId,
                defaultWarehouse.Id);

            if (reservedStock < quantity)
                throw new Exception($"Not enough reserved stock to release. Reserved: {reservedStock}, Requested: {quantity}");

            // آزادسازی موجودی رزرو شده
            await _warehouseService.ReleaseReservedStockAsync(
                productId,
                variantId,
                defaultWarehouse.Id,
                quantity);
        }

        public Task AdjustStockAsync(long productId, long? variantId, int adjustment, string reason)
        {
            throw new NotImplementedException();
        }
    }
}