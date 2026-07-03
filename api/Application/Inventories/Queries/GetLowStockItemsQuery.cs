using Application.Cqrs.Queris;
using Application.Warehouseing.DTOs.Application.Inventory.DTOs;
using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Inventories.Queries
{
    public class GetLowStockItemsQuery(long? warehouseId)
        : IQuery<ServiceResult<List<LowStockItemDTO>>>
    {
        public long? WarehouseId { get; } = warehouseId;
    }

    public class GetLowStockItemsQueryHandler(
        IRepository<Inventory> inventoryRepository,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : IQueryHandler<GetLowStockItemsQuery, ServiceResult<List<LowStockItemDTO>>>
    {
        public async Task<ServiceResult<List<LowStockItemDTO>>> Handle(
            GetLowStockItemsQuery request,
            CancellationToken cancellationToken)
        {
            var query = inventoryRepository.TableNoTracking
                .Include(i => i.Warehouse)
                .Include(i => i.Product)
                .Include(i => i.Variant)
                .Where(i => i.AvailableQuantity <= i.ReorderPoint);

            if (request.WarehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == request.WarehouseId.Value);

            var lowStockItems = await query
                .OrderBy(i => i.AvailableQuantity)
                .ThenBy(i => i.Product.Name)
                .Select(i => new LowStockItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    VariantId = i.VariantId,
                    VariantName = i.Variant != null ? i.Variant.GetVariantIdentifier() : null,
                    AvailableQuantity = i.AvailableQuantity,
                    ReorderPoint = i.ReorderPoint,
                    WarehouseName = i.Warehouse.Name
                })
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(lowStockItems);
        }
    }
}