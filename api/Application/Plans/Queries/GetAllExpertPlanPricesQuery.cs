using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetAllExpertPlanPricesQuery(GlobalGrid GlobalGrid, long? planId) : IQuery<ServiceResult<GlobalGridResult<ExpertPlanPriceDTO>>>;

    public class GetAllExpertPlanPricesQueryHandler : IQueryHandler<GetAllExpertPlanPricesQuery, ServiceResult<GlobalGridResult<ExpertPlanPriceDTO>>>
    {
        private readonly IRepository<ExpertPlanPrice> _repository;

        public GetAllExpertPlanPricesQueryHandler(IRepository<ExpertPlanPrice> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<GlobalGridResult<ExpertPlanPriceDTO>>> Handle(GetAllExpertPlanPricesQuery request, CancellationToken cancellationToken)
        {
            var query = _repository
                .Table
                .Include(p => p.Expert)
                .Include(p => p.Plan)
                .AsQueryable();
            if (request.planId.HasValue)
                query = query.Where(t => t.PlanId == request.planId.Value);

            var data = await query
     .Select(p => new ExpertPlanPriceDTO
     {
         ExpertId = p.ExpertId,
         ExpertFullName = p.Expert != null ? p.Expert.FullName ?? "-" : "-",
         PlanId = p.PlanId,
         PlanTitle = p.Plan != null ? p.Plan.Title : "-",
         Price = p.Price,
         IsActive = p.IsActive

     })
     .Skip(request.GlobalGrid.Skip)
     .Take(request.GlobalGrid.Take)
     .ToListAsync(cancellationToken);


            return ServiceResult.Ok(new GlobalGridResult<ExpertPlanPriceDTO>
            {
                Data = data,

                TotalCount = await query.CountAsync(cancellationToken)

            });
        }
    }
}
