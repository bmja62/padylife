using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetExpertPlanPriceQuery(long Id, bool? isActive) : IQuery<ServiceResult<ExpertPlanPriceDTO>>;

    public class GetExpertPlanPriceQueryHandler : IQueryHandler<GetExpertPlanPriceQuery, ServiceResult<ExpertPlanPriceDTO>>
    {
        private readonly IRepository<ExpertPlanPrice> _priceRepository;

        public GetExpertPlanPriceQueryHandler(IRepository<ExpertPlanPrice> priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<ServiceResult<ExpertPlanPriceDTO>> Handle(GetExpertPlanPriceQuery request, CancellationToken cancellationToken)
        {
            var data = await _priceRepository
                .Table
                .Where(t => request.isActive.HasValue ? t.IsActive == request.isActive.Value : true)
                .Where(p => p.ExpertId == request.Id)
                .Include(p => p.Plan)
                .Select(p => new ExpertPlanPriceDTO
                {
                    PlanId = p.PlanId,
                    PlanTitle = p.Plan.Title,
                    Price = p.Price,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            return ServiceResult.Ok<ExpertPlanPriceDTO>(data);
        }
    }
}
