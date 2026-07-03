using Common;
using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.Warehousing.WarehouseServices
{
    public interface IWarehouseService
    {
        // Warehouse operations
        Task<Warehouse> CreateWarehouseAsync(string name, string code, string address);
        Task<WarehouseZone> AddZoneToWarehouseAsync(long warehouseId, string name, string code, int capacity);

        // Inventory operations
        Task<Inventory> GetOrCreateInventoryAsync(long productId, long? variantId, long warehouseId, long? zoneId = null);
        Task<Inventory> UpdateInventorySettingsAsync(long inventoryId, int? minimumStock, int? reorderPoint);

        // Stock operations
        Task IncreaseStockAsync(long productId, long? variantId, long warehouseId, int quantity,
            InventoryTransactionType type, string reason, string referenceId = null);
        Task DecreaseStockAsync(long productId, long? variantId, long warehouseId, int quantity,
            InventoryTransactionType type, string reason, string referenceId = null);
        Task ReserveStockAsync(long productId, long? variantId, long warehouseId, int quantity);
        Task ReleaseReservedStockAsync(long productId, long? variantId, long warehouseId, int quantity);
        Task TransferStockAsync(long productId, long? variantId, long fromWarehouseId, long toWarehouseId,
            int quantity, string reason);

        // Query operations
        Task<int> GetAvailableStockAsync(long productId, long? variantId, long? warehouseId = null);
        Task<int> GetReservedStockAsync(long productId, long? variantId, long? warehouseId = null);
        Task<List<Inventory>> GetLowStockItemsAsync(long? warehouseId = null);
        Task<List<InventoryTransaction>> GetInventoryHistoryAsync(long productId, long? variantId,
            DateTime? fromDate = null, DateTime? toDate = null);
        Task<Warehouse> GetDefaultWarehouseAsync();
        Task<Warehouse> GetWarehouseAsync(long id);

    }

    public class WarehouseService : IWarehouseService, IScopedDependency
    {
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<InventoryTransaction> _transactionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _variantRepository;

        public WarehouseService(
            IRepository<Warehouse> warehouseRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<InventoryTransaction> transactionRepository,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> variantRepository
            )
        {
            _warehouseRepository = warehouseRepository;
            _inventoryRepository = inventoryRepository;
            _transactionRepository = transactionRepository;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
        }

        public async Task<Warehouse> CreateWarehouseAsync(string name, string code, string address)
        {
            if (await _warehouseRepository.Table.AnyAsync(w => w.Code == code, CancellationToken.None))
                throw new InvalidOperationException("Warehouse with this code already exists");

            var warehouse = new Warehouse
            {
                Name = name,
                Code = code,
                Address = address,
                IsActive = true
            };

            await _warehouseRepository.AddAsync(warehouse, CancellationToken.None);
            return warehouse;
        }

        public async Task<WarehouseZone> AddZoneToWarehouseAsync(long warehouseId, string name, string code, int capacity)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(CancellationToken.None, warehouseId);
            if (warehouse == null)
                throw new InvalidOperationException("Warehouse not found");

            if (warehouse.Zones.Any(z => z.Code == code))
                throw new InvalidOperationException("Zone with this code already exists in this warehouse");

            var zone = warehouse.AddZone(name, code, capacity);
            await _warehouseRepository.UpdateAsync(warehouse, CancellationToken.None);
            return zone;
        }

        public async Task<Inventory> GetOrCreateInventoryAsync(long productId, long? variantId, long warehouseId, long? zoneId = null)
        {
            var query = _inventoryRepository.Table;

            if (productId > 0)
                query = query.Where(i => i.ProductId == productId);

            if (variantId != null && variantId.HasValue && variantId.Value > 0)
                query = query.Where(i => i.VariantId == variantId);

            if (warehouseId > 0)
                query = query.Where(i => i.WarehouseId == warehouseId);

            if (zoneId != null && zoneId.HasValue && zoneId > 0)
                query = query.Where(i => i.ZoneId == zoneId.Value);

            var inventory = await query.FirstOrDefaultAsync();

            if (inventory == null)
            {
                // Validate product/variant
                var product = await _productRepository.GetByIdAsync(CancellationToken.None, productId);
                if (product == null)
                    throw new InvalidOperationException("Product not found");

                if (variantId.HasValue)
                {
                    var variant = await _variantRepository.GetByIdAsync(CancellationToken.None, variantId.Value);
                    if (variant == null || variant.ProductId != productId)
                        throw new InvalidOperationException("Variant not found for this product");
                }

                // Validate warehouse/zone
                var warehouse = await _warehouseRepository.GetByIdAsync(CancellationToken.None, warehouseId);
                if (warehouse == null)
                    throw new InvalidOperationException("Warehouse not found");

                if (zoneId.HasValue && !warehouse.Zones.Any(z => z.Id == zoneId.Value))
                    throw new InvalidOperationException("Zone not found in this warehouse");

                inventory = new Inventory
                {
                    ProductId = productId,
                    VariantId = variantId,
                    WarehouseId = warehouseId,
                    ZoneId = zoneId,
                    Quantity = 1,
                    ReservedQuantity = 0,
                    MinimumStock = 0,
                    ReorderPoint = 0
                };

                await _inventoryRepository.AddAsync(inventory, CancellationToken.None);
            }

            return inventory;
        }

        public async Task<Inventory> UpdateInventorySettingsAsync(long inventoryId, int? minimumStock, int? reorderPoint)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(CancellationToken.None, inventoryId);
            if (inventory == null)
                throw new InvalidOperationException("Inventory not found");

            if (minimumStock.HasValue)
                inventory.MinimumStock = minimumStock.Value;

            if (reorderPoint.HasValue)
                inventory.ReorderPoint = reorderPoint.Value;

            await _inventoryRepository.UpdateAsync(inventory, CancellationToken.None);
            return inventory;
        }

        public async Task IncreaseStockAsync(long productId, long? variantId, long warehouseId, int quantity,
            InventoryTransactionType type, string reason, string referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            var inventory = await GetOrCreateInventoryAsync(productId, variantId, warehouseId);
            inventory.AdjustStock(quantity, reason, type);

            if (!string.IsNullOrEmpty(referenceId))
                inventory.Transactions.Last().ReferenceId = referenceId;

            await _inventoryRepository.UpdateAsync(inventory, CancellationToken.None);
        }

        public async Task DecreaseStockAsync(long productId, long? variantId, long warehouseId, int quantity,
            InventoryTransactionType type, string reason, string referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            var inventory = await GetOrCreateInventoryAsync(productId, variantId, warehouseId);
            inventory.AdjustStock(-quantity, reason, type);

            if (!string.IsNullOrEmpty(referenceId))
                inventory.Transactions.Last().ReferenceId = referenceId;

            await _inventoryRepository.UpdateAsync(inventory, CancellationToken.None);
        }

        public async Task ReserveStockAsync(long productId, long? variantId, long warehouseId, int quantity)
        {
            var inventory = await GetOrCreateInventoryAsync(productId, variantId, warehouseId);

            if (inventory.AvailableQuantity < quantity)
                throw new InvalidOperationException("Not enough available stock to reserve");

            inventory.ReserveStock(quantity);
            await _inventoryRepository.UpdateAsync(inventory, CancellationToken.None);

            await AddTransactionAsync(inventory, InventoryTransactionType.Reservation, -quantity,
                $"Stock reservation for product {productId}, variant {variantId}");
        }

        public async Task ReleaseReservedStockAsync(long productId, long? variantId, long warehouseId, int quantity)
        {
            var inventory = await GetOrCreateInventoryAsync(productId, variantId, warehouseId);

            if (inventory.ReservedQuantity < quantity)
                throw new InvalidOperationException("Not enough reserved stock to release");

            inventory.ReleaseReservedStock(quantity);
            await _inventoryRepository.UpdateAsync(inventory, CancellationToken.None);

            await AddTransactionAsync(inventory, InventoryTransactionType.Release, quantity,
                $"Release reserved stock for product {productId}, variant {variantId}");
        }

        public async Task TransferStockAsync(long productId, long? variantId, long fromWarehouseId,
            long toWarehouseId, int quantity, string reason)
        {
            if (fromWarehouseId == toWarehouseId)
                throw new InvalidOperationException("Source and destination warehouses cannot be the same");

            var sourceInventory = await GetOrCreateInventoryAsync(productId, variantId, fromWarehouseId);
            var targetInventory = await GetOrCreateInventoryAsync(productId, variantId, toWarehouseId);

            if (sourceInventory.AvailableQuantity < quantity)
                throw new InvalidOperationException("Not enough available stock in source warehouse");

            sourceInventory.DecreaseStock(quantity, reason, InventoryTransactionType.TransferOut);
            targetInventory.IncreaseStock(quantity, reason, InventoryTransactionType.TransferIn);

            // Set warehouse references for the transactions
            sourceInventory.Transactions.Last().SourceWarehouseId = fromWarehouseId;
            sourceInventory.Transactions.Last().DestinationWarehouseId = toWarehouseId;
            targetInventory.Transactions.Last().SourceWarehouseId = fromWarehouseId;
            targetInventory.Transactions.Last().DestinationWarehouseId = toWarehouseId;

            await _inventoryRepository.UpdateAsync(sourceInventory, CancellationToken.None);
            await _inventoryRepository.UpdateAsync(targetInventory, CancellationToken.None);
        }

        public async Task<int> GetAvailableStockAsync(long productId, long? variantId, long? warehouseId = null)
        {
            var query = _inventoryRepository.Table
                .Where(i => i.ProductId == productId && i.VariantId == variantId);

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            return await query.SumAsync(i => i.Quantity - i.ReservedQuantity, CancellationToken.None);
        }

        public async Task<int> GetReservedStockAsync(long productId, long? variantId, long? warehouseId = null)
        {
            var query = _inventoryRepository.Table
                .Where(i => i.ProductId == productId && i.VariantId == variantId);

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            return await query.SumAsync(i => i.ReservedQuantity, CancellationToken.None);
        }

        public async Task<List<Inventory>> GetLowStockItemsAsync(long? warehouseId = null)
        {
            var query = _inventoryRepository.Table
                .Include(i => i.Product)
                .Include(i => i.Variant)
                .Include(i => i.Warehouse)
                .Where(i => i.AvailableQuantity <= i.ReorderPoint);

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<InventoryTransaction>> GetInventoryHistoryAsync(long productId, long? variantId,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _transactionRepository.Table
                .Include(t => t.Inventory)
                .Where(t => t.Inventory.ProductId == productId &&
                           t.Inventory.VariantId == variantId)
                .OrderByDescending(t => t.TransactionDate).AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value);

            return await query.ToListAsync();
        }

        public async Task<Warehouse> GetDefaultWarehouseAsync()
        {

            var warehouse = await _warehouseRepository.Table.FirstOrDefaultAsync(CancellationToken.None);
            if (warehouse == null)
                throw new InvalidOperationException("Configured default warehouse not found");

            return warehouse;
        }

        private async Task AddTransactionAsync(Inventory inventory, InventoryTransactionType type,
            int quantity, string description)
        {
            var transaction = new InventoryTransaction
            {
                InventoryId = inventory.Id,
                Type = type,
                Quantity = quantity,
                Description = description,
                TransactionDate = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction, CancellationToken.None);
        }

        public async Task<Warehouse> GetWarehouseAsync(long id)
        {
            var warehouse = await _warehouseRepository.Table.Where(t => t.Id == id).FirstOrDefaultAsync(CancellationToken.None);
            if (warehouse == null)
                throw new InvalidOperationException("Configured default warehouse not found");

            return warehouse;
        }

    }
}