using Application.Cqrs.Queris;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Warehouseing.Queries
{
    public class GetWarehousesQuery : IQuery<ServiceResult<GlobalGridResult<WarehouseDTO>>>
    {
        public GetWarehousesQuery(GlobalGrid globalGrid, bool? isActive)
        {
            GlobalGrid = globalGrid;
            IsActive = isActive;
        }
        public GlobalGrid GlobalGrid { get; }
        public bool? IsActive { get; }
    }

    public class GetWarehousesQueryHandler(
        IRepository<Warehouse> warehouseRepository)
        : IQueryHandler<GetWarehousesQuery, ServiceResult<GlobalGridResult<WarehouseDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<WarehouseDTO>>> Handle(
            GetWarehousesQuery request,
            CancellationToken cancellationToken)
        {
            var query = warehouseRepository.TableNoTracking;

            if (request.IsActive.HasValue)
                query = query.Where(w => w.IsActive == request.IsActive.Value);

            var warehouses = await query
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseDTO
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Address = w.Address,
                    IsActive = w.IsActive,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude
                })
                .Skip(request.GlobalGrid.Skip)
                .Take(request.GlobalGrid.Take)
                .ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<WarehouseDTO>
            {
                Data = warehouses,
                TotalCount = totalCount
            });
        }
    }
}