using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Services.Services.StockManagerServices;

namespace Services.DataInitializer
{
    public class WarehouseInitializer(
        IRepository<Warehouse> warehouseRepo,
        IRepository<Inventory> inventoryRepo,
        IRepository<Product> productRepo,
        IRepository<ProductVariant> variantRepo,
        IStockManagerService stockManagerService) : IDataInitializer
    {
        public void InitializeData()
        {
            if (warehouseRepo.Table.Any())
                return;

            // 1. ایجاد انبارهای اولیه
            var mainWarehouse = new Warehouse
            {
                Name = "انبار مرکزی",
                Code = "WH-001",
                Address = "تهران، خیابان آزادی، پلاک 100",
                ContactPhone = "02112345678",
                ManagerName = "محمد رضایی",
                Latitude = 35.7000m,
                Longitude = 51.4000m,
                IsActive = true
            };
            warehouseRepo.Add(mainWarehouse);

            var northWarehouse = new Warehouse
            {
                Name = "انبار شمال",
                Code = "WH-002",
                Address = "مازندران، ساری، بلوار طالقانی",
                ContactPhone = "01112345678",
                ManagerName = "علی محمدی",
                Latitude = 36.5661m,
                Longitude = 53.0586m,
                IsActive = true
            };
            warehouseRepo.Add(northWarehouse);

            // 2. ایجاد مناطق در انبارها
            var mainZones = new List<WarehouseZone>
            {
                new() { Name = "منطقه A", Code = "ZONE-A", Capacity = 1000, Warehouse = mainWarehouse },
                new() { Name = "منطقه B", Code = "ZONE-B", Capacity = 800, Warehouse = mainWarehouse },
                new() { Name = "منطقه یخچالی", Code = "ZONE-COLD", Capacity = 200, Warehouse = mainWarehouse }
            };

            var northZones = new List<WarehouseZone>
            {
                new() { Name = "منطقه شرقی", Code = "ZONE-EAST", Capacity = 500, Warehouse = northWarehouse },
                new() { Name = "منطقه غربی", Code = "ZONE-WEST", Capacity = 500, Warehouse = northWarehouse }
            };

            mainWarehouse.Zones = mainZones;
            northWarehouse.Zones = northZones;
            warehouseRepo.UpdateRange(new[] { mainWarehouse, northWarehouse });

            // 3. ایجاد موجودی اولیه برای محصولات
            var products = productRepo.Table.ToList();
            var variants = variantRepo.Table.ToList();

            // ایجاد موجودی برای محصولات ساده
            foreach (var product in products.Where(p => p.Type == ProductType.Simple))
            {
                AddInitialInventory(product, null, mainWarehouse, mainZones.First());
                AddInitialInventory(product, null, northWarehouse, northZones.First());
            }

            // ایجاد موجودی برای واریانت‌های محصولات متغیر
            foreach (var variant in variants)
            {
                AddInitialInventory(variant.Product, variant, mainWarehouse, mainZones.First());
                AddInitialInventory(variant.Product, variant, northWarehouse, northZones.First());
            }
        }

        private void AddInitialInventory(Product product, ProductVariant variant, Warehouse warehouse, WarehouseZone zone)
        {
            var random = new Random();
            var quantity = random.Next(5, 50); // موجودی تصادفی بین 5 تا 50
            var reorderPoint = (int)(quantity * 0.3); // نقطه سفارش مجدد 30% موجودی

            var inventory = new Inventory
            {
                ProductId = product.Id,
                VariantId = variant?.Id,
                WarehouseId = warehouse.Id,
                ZoneId = zone.Id,
                Quantity = quantity,
                ReservedQuantity = 0,
                MinimumStock = 5,
                ReorderPoint = reorderPoint,
                LastStockUpdate = DateTime.UtcNow
            };

            inventoryRepo.Add(inventory);

            // ثبت تراکنش اولیه
            var transaction = new InventoryTransaction
            {
                Inventory = inventory,
                Type = InventoryTransactionType.Adjustment,
                Quantity = quantity,
                Description = "موجودی اولیه سیستم",
                TransactionDate = DateTime.UtcNow,
                PerformedBy = "System"
            };

            inventory.Transactions = new List<InventoryTransaction> { transaction };
            inventoryRepo.Update(inventory);
        }
    }
}