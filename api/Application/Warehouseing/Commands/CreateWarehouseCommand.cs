using Application.Cqrs.Commands;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Data.Contracts;
using Entities.Warehouseing;
using Services;

namespace Application.Warehouseing.Commands
{
    public class CreateWarehouseCommand : ICommand<ServiceResult<WarehouseDTO>>
    {
        public CreateWarehouseCommand(CreateWarehouseDTO input)
        {
            Input = input;
        }

        public CreateWarehouseDTO Input { get; }
    }
    public class CreateWarehouseCommandHandler(
      IRepository<Warehouse> warehouseRepository)
      : ICommandHandler<CreateWarehouseCommand, ServiceResult<WarehouseDTO>>
    {
        public async Task<ServiceResult<WarehouseDTO>> Handle(
            CreateWarehouseCommand request,
            CancellationToken cancellationToken)
        {
            var warehouse = new Warehouse
            {
                Name = request.Input.Name,
                Code = request.Input.Code,
                Address = request.Input.Address,
                ContactPhone = request.Input.ContactPhone,
                ManagerName = request.Input.ManagerName,
                Latitude = request.Input.Latitude,
                Longitude = request.Input.Longitude,
                IsActive = true
            };

            await warehouseRepository.AddAsync(warehouse, cancellationToken);

            return ServiceResult.Ok(new WarehouseDTO
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Address = warehouse.Address
            });
        }
    }
}