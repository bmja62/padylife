using Application.Cqrs.Commands;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Data.Contracts;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Commands
{
    public class AddZoneCommand : ICommand<ServiceResult<WarehouseZoneDTO>>
    {
        public AddZoneCommand(long warehouseId, AddZoneDTO input)
        {
            WarehouseId = warehouseId;
            Input = input;
        }

        public long WarehouseId { get; }
        public AddZoneDTO Input { get; }
    }

    public class AddZoneCommandHandler(
      IRepository<Warehouse> warehouseRepository)
      : ICommandHandler<AddZoneCommand, ServiceResult<WarehouseZoneDTO>>
    {
        public async Task<ServiceResult<WarehouseZoneDTO>> Handle(
            AddZoneCommand request,
            CancellationToken cancellationToken)
        {
            var warehouse = await warehouseRepository.Table
                .Include(w => w.Zones)
                .FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken);

            if (warehouse == null)
                return ServiceResult.Fail<WarehouseZoneDTO>(null, "انبار مورد نظر یافت نشد", Common.ApiResultStatusCode.NotFound);

            // بررسی تکراری نبودن کد منطقه در این انبار
            if (warehouse.Zones.Any(z => z.Code == request.Input.Code))
                return ServiceResult.Fail<WarehouseZoneDTO>(null, "کد منطقه تکراری است", Common.ApiResultStatusCode.BadRequest);

            var zone = warehouse.AddZone(request.Input.Name, request.Input.Code, request.Input.Capacity);
            await warehouseRepository.UpdateAsync(warehouse, cancellationToken);

            return ServiceResult.Ok(new WarehouseZoneDTO
            {
                Id = zone.Id,
                Name = zone.Name,
                Code = zone.Code,
                Capacity = zone.Capacity,
                CurrentOccupancy = 0 // در ابتدا منطقه خالی است
            });
        }
    }
}