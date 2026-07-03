using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.Queries
{
    public record GetPlanDiscountQuery(long planId) : IQuery<ServiceResult<SetPlanDiscountDTO>>;
    public class GetPlanDiscountQueryHandler(IRepository<Plan> repository) : IQueryHandler<GetPlanDiscountQuery, ServiceResult<SetPlanDiscountDTO>>
    {
        public async Task<ServiceResult<SetPlanDiscountDTO>> Handle(GetPlanDiscountQuery request, CancellationToken cancellationToken) => ServiceResult.Ok(await repository.Table.Where(t => t.Id == request.planId).Select(t => new SetPlanDiscountDTO
        {
            DiscountPrice = t.DiscountPrice,
            DiscountPriceEndDate = t.DiscountPriceEndDate,
            DiscountPriceStartDate = t.DiscountPriceStartDate,
            PlanId = t.Id,
        }).FirstOrDefaultAsync(cancellationToken));
    }
}
