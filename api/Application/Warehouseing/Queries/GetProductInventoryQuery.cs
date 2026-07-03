using Application.Cqrs.Queris;
using Application.Warehouseing.DTOs;
using Application.Warehouseing.DTOs.Application.Inventory.DTOs;
using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Queries
{
    public class GetProductInventoryQuery(long productId, long? variantId)
        : IQuery<ServiceResult<GetProductInventoryDTO>>
    {
        public long ProductId { get; } = productId;
        public long? VariantId { get; } = variantId;
    }

    public class GetProductInventoryQueryHandler(
        IRepository<Inventory> inventoryRepository,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : IQueryHandler<GetProductInventoryQuery, ServiceResult<GetProductInventoryDTO>>
    {
        public async Task<ServiceResult<GetProductInventoryDTO>> Handle(
            GetProductInventoryQuery request,
            CancellationToken cancellationToken)
        {
            // بررسی وجود محصول
            var productExists = await productRepository.TableNoTracking
                .AnyAsync(p => p.Id == request.ProductId, cancellationToken);

            if (!productExists)
                return ServiceResult.Fail<GetProductInventoryDTO>(null, "محصول مورد نظر یافت نشد", Common.ApiResultStatusCode.NotFound);

            // بررسی وجود واریانت اگر مشخص شده باشد
            if (request.VariantId.HasValue)
            {
                var variantExists = await variantRepository.TableNoTracking
                    .AnyAsync(v => v.Id == request.VariantId && v.ProductId == request.ProductId,
                        cancellationToken);

                if (!variantExists)
                    return ServiceResult.Fail<GetProductInventoryDTO>(null, "واریانت مورد نظر برای این محصول یافت نشد", Common.ApiResultStatusCode.NotFound);
            }

            var query = inventoryRepository.TableNoTracking
                .Include(i => i.Warehouse)
                .Include(i => i.Zone)
                .Include(i => i.Product)
                .Include(i => i.Variant)
                .ThenInclude(t => t.AttributeValues)
                    .ThenInclude(t => t.Attribute)
                    .AsQueryable();

            if (request.ProductId >= 0)
                query = query.Where(i => i.ProductId == request.ProductId);

            if (request.VariantId.HasValue && request.VariantId.Value >= 0)
                query = query.Where(i => i.VariantId == request.VariantId);


            // محاسبه مجموع موجودی در تمام انبارها
            var totalQuantity = await query
                .SumAsync(i => i.Quantity, cancellationToken);

            var totalReserved = await query
                .SumAsync(i => i.ReservedQuantity, cancellationToken);

            // دریافت لیست موجودی‌ها با اطلاعات انبار
            var inventories = await query
                .OrderBy(i => i.Warehouse.Name)
                .ThenBy(i => i.Zone != null ? i.Zone.Name : "")
                .Select(i => new ProductInventoryDTO
                {
                    WarehouseId = i.WarehouseId,
                    WarehouseName = i.Warehouse.Name,
                    ZoneId = i.ZoneId,
                    ZoneName = i.Zone != null ? i.Zone.Name : null,
                    Quantity = i.Quantity,
                    ReservedQuantity = i.ReservedQuantity,
                    AvailableQuantity = i.Quantity - i.ReservedQuantity,
                    MinimumStock = i.MinimumStock,
                    ReorderPoint = i.ReorderPoint,
                    LastStockUpdate = i.LastStockUpdate,
                    ProductName = i.Product.Name,
                    VariantName = i.Variant != null ? i.Variant.GetVariantIdentifier() : null
                })
                .ToListAsync(cancellationToken);

            // محاسبه اطلاعات خلاصه
            var summary = new InventorySummaryDTO
            {
                TotalQuantity = totalQuantity,
                TotalReserved = totalReserved,
                TotalAvailable = totalQuantity - totalReserved,
                WarehouseCount = inventories.Select(i => i.WarehouseId).Distinct().Count(),
                ZoneCount = inventories.Select(i => i.ZoneId).Distinct().Count(z => z.HasValue)
            };


            var data = inventories;
            var totalCount = inventories.Count;

            return ServiceResult.Ok(new GetProductInventoryDTO
            {
                Data = data,
                TotalCount = totalCount,
                Summery = summary
            });
        }
    }
}