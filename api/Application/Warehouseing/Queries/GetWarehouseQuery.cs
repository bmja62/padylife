using Application.Cqrs.Queris;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Data.Contracts;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Queries
{
    public class GetWarehouseQuery(long id) : IQuery<ServiceResult<WarehouseDetailDTO>>
    {
        public long Id { get; } = id;
    }

    public class GetWarehouseQueryHandler(
        IRepository<Warehouse> warehouseRepository)
        : IQueryHandler<GetWarehouseQuery, ServiceResult<WarehouseDetailDTO>>
    {
        public async Task<ServiceResult<WarehouseDetailDTO>> Handle(
            GetWarehouseQuery request,
            CancellationToken cancellationToken)
        {
            var warehouse = await warehouseRepository.TableNoTracking
                .Include(w => w.Zones)
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (warehouse == null)
                return ServiceResult.Fail<WarehouseDetailDTO>(null, "انبار مورد نظر یافت نشد", Common.ApiResultStatusCode.NotFound);

            var result = new WarehouseDetailDTO
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Address = warehouse.Address,
                ContactPhone = warehouse.ContactPhone,
                ManagerName = warehouse.ManagerName,
                Latitude = warehouse.Latitude,
                Longitude = warehouse.Longitude,
                IsActive = warehouse.IsActive,
                Zones = warehouse.Zones.Select(z => new WarehouseZoneDTO
                {
                    Id = z.Id,
                    Name = z.Name,
                    Code = z.Code,
                    Capacity = z.Capacity,
                    CurrentOccupancy = 0 // این مقدار در عمل باید از موجودی‌ها محاسبه شود
                }).ToList()
            };

            return ServiceResult.Ok(result);
        }
    }
}