using Application.Cqrs.Commands;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Data.Contracts;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Commands
{
    public class UpdateWarehouseCommand : ICommand<ServiceResult>
    {
        public UpdateWarehouseCommand(long id, UpdateWarehouseDTO input)
        {
            Id = id;
            Input = input;
        }

        public long Id { get; }
        public UpdateWarehouseDTO Input { get; }
    }

    public class UpdateWarehouseCommandHandler(
      IRepository<Warehouse> warehouseRepository)
      : ICommandHandler<UpdateWarehouseCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            UpdateWarehouseCommand request,
            CancellationToken cancellationToken)
        {
            var warehouse = await warehouseRepository.GetByIdAsync(cancellationToken, request.Id);
            if (warehouse == null)
                return ServiceResult.Fail("انبار مورد نظر یافت نشد");

            // بررسی تکراری نبودن کد انبار
            if (await warehouseRepository.Table
                .AnyAsync(w => w.Code == request.Input.Code && w.Id != request.Id, cancellationToken))
                return ServiceResult.Fail("کد انبار تکراری است");

            warehouse.Name = request.Input.Name;
            warehouse.Code = request.Input.Code;
            warehouse.Address = request.Input.Address;
            warehouse.ContactPhone = request.Input.ContactPhone;
            warehouse.ManagerName = request.Input.ManagerName;
            warehouse.Latitude = request.Input.Latitude;
            warehouse.Longitude = request.Input.Longitude;
            warehouse.IsActive = request.Input.IsActive;

            await warehouseRepository.UpdateAsync(warehouse, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}