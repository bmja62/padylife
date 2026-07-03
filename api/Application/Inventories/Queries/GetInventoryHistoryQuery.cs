using Application.Cqrs.Queris;
using Application.Warehouseing.DTOs.Application.Inventory.DTOs;
using Data.Contracts;
using Entities.Products;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Inventories.Queries
{
    public class GetInventoryHistoryQuery(
         long productId,
         long? variantId,
         DateTime? fromDate,
         DateTime? toDate)
         : IQuery<ServiceResult<List<InventoryTransactionDTO>>>
    {
        public long ProductId { get; } = productId;
        public long? VariantId { get; } = variantId;
        public DateTime? FromDate { get; } = fromDate;
        public DateTime? ToDate { get; } = toDate;
    }

    public class GetInventoryHistoryQueryHandler(
        IRepository<InventoryTransaction> transactionRepository,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : IQueryHandler<GetInventoryHistoryQuery, ServiceResult<List<InventoryTransactionDTO>>>
    {
        public async Task<ServiceResult<List<InventoryTransactionDTO>>> Handle(
            GetInventoryHistoryQuery request,
            CancellationToken cancellationToken)
        {
            // بررسی وجود محصول
            var productExists = await productRepository.TableNoTracking
                .AnyAsync(p => p.Id == request.ProductId, cancellationToken);

            if (!productExists)
                return ServiceResult.NotFound<List<InventoryTransactionDTO>>("محصول مورد نظر یافت نشد");

            // بررسی وجود واریانت اگر مشخص شده باشد
            if (request.VariantId.HasValue)
            {
                var variantExists = await variantRepository.TableNoTracking
                    .AnyAsync(v => v.Id == request.VariantId && v.ProductId == request.ProductId,
                        cancellationToken);

                if (!variantExists)
                    return ServiceResult.NotFound<List<InventoryTransactionDTO>>("واریانت مورد نظر برای این محصول یافت نشد");
            }

            var query = transactionRepository.TableNoTracking
                .Include(t => t.Inventory)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Where(t => t.Inventory.ProductId == request.ProductId &&
                           t.Inventory.VariantId == request.VariantId)
                .OrderByDescending(t => t.TransactionDate).AsQueryable();

            if (request.FromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.TransactionDate <= request.ToDate.Value);

            var transactions = await query
                .Select(t => new InventoryTransactionDTO
                {
                    TransactionDate = t.TransactionDate,
                    Type = t.Type.ToString(),
                    Quantity = t.Quantity,
                    Description = t.Description,
                    ReferenceId = t.ReferenceId,
                    WarehouseName = t.SourceWarehouseId != null ?
                        t.SourceWarehouse.Name :
                        t.DestinationWarehouse.Name,
                })
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(transactions);
        }
    }
}