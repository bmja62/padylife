using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Commands
{
    public class DeactivateWarehouseCommand : ICommand<ServiceResult>
    {
        public DeactivateWarehouseCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class DeactivateWarehouseCommandHandler(
      IRepository<Warehouse> warehouseRepository,
      IRepository<Inventory> inventoryRepository)
      : ICommandHandler<DeactivateWarehouseCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            DeactivateWarehouseCommand request,
            CancellationToken cancellationToken)
        {
            var warehouse = await warehouseRepository.GetByIdAsync(cancellationToken, request.Id);
            if (warehouse == null)
                return ServiceResult.Fail("انبار مورد نظر یافت نشد");

            // بررسی وجود موجودی در انبار
            var hasInventory = await inventoryRepository.Table
                .AnyAsync(i => i.WarehouseId == request.Id && i.Quantity > 0, cancellationToken);

            if (hasInventory)
                return ServiceResult.Fail("امکان غیرفعال کردن انبار با موجودی وجود ندارد");

            warehouse.IsActive = false;
            await warehouseRepository.UpdateAsync(warehouse, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}