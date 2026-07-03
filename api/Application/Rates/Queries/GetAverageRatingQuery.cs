using Application.Cqrs.Queris;
using Application.Rates.DTOs;
using Entities.Common;
using Services;
using Services.Services.RateServices;

namespace Application.Rates.Queries
{
    public class GetAverageRatingQuery(long entityId, EntityType entityType) : IQuery<ServiceResult<GetAverageRatingDTO>>
    {
        public long EntityId { get; } = entityId;
        public EntityType EntityType { get; } = entityType;
    }
    public class GetAverageRatingQueryHandler(IRateService rateService) : IQueryHandler<GetAverageRatingQuery, ServiceResult<GetAverageRatingDTO>>
    {
        public async Task<ServiceResult<GetAverageRatingDTO>> Handle(GetAverageRatingQuery request, CancellationToken cancellationToken)
        => ServiceResult.Ok(new GetAverageRatingDTO
        {
            Avg = await rateService.GetAverageRatingAsync(request.EntityId, request.EntityType)
        });

    }
}
