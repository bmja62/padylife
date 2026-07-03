using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetExpertPlanPriceForUIQuery(long expretId, long planId, bool? isActive) : IQuery<ServiceResult<ExpertPlanPriceDTO>>;
    public class GetExpertPlanPriceForUIQueryHandler : IQueryHandler<GetExpertPlanPriceForUIQuery, ServiceResult<ExpertPlanPriceDTO>>
    {
        private readonly IRepository<ExpertPlanPrice> _priceRepository;

        public GetExpertPlanPriceForUIQueryHandler(IRepository<ExpertPlanPrice> priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<ServiceResult<ExpertPlanPriceDTO>> Handle(GetExpertPlanPriceForUIQuery request, CancellationToken cancellationToken)
        {

            var query = _priceRepository.Table;
            var data = await query
                .Where(t => request.isActive.HasValue ? t.IsActive == request.isActive.Value : true)
                .Where(p => p.ExpertId == request.expretId)
                .Where(p => p.PlanId == request.planId)
                .Include(p => p.Plan)
                .Select(p => new ExpertPlanPriceDTO
                {
                    Id = p.Id,
                    ExpertId = p.ExpertId,
                    ExpertFullName = p.Expert.FullName,
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
