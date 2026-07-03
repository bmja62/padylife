using Application.Cqrs.Queris;
using Services;
using Services.Services.Visits;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetGrowthStatsQuery.cs
    public class GetGrowthStatsQuery : IQuery<ServiceResult<GrowthStatsDTO>>
    {
        public string EntityType { get; }
        public long? EntityId { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetGrowthStatsQuery(string entityType, long? entityId, DateTime? fromDate, DateTime? toDate)
        {
            EntityType = entityType;
            EntityId = entityId;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetGrowthStatsQueryHandler
        : IQueryHandler<GetGrowthStatsQuery, ServiceResult<GrowthStatsDTO>>
    {
        private readonly IVisitTrackingService _visitTrackingService;

        public GetGrowthStatsQueryHandler(IVisitTrackingService visitTrackingService)
        {
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult<GrowthStatsDTO>> Handle(
            GetGrowthStatsQuery request, CancellationToken cancellationToken)
        {
            var entityStats = await _visitTrackingService.GetEntityStatsAsync(
                request.EntityType, request.EntityId, request.FromDate, request.ToDate);

            var dto = new GrowthStatsDTO
            {
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                GrowthRate = entityStats.GrowthRate,
                CurrentPeriodVisits = entityStats.TotalVisits,
                PreviousPeriodVisits = await GetPreviousPeriodVisitsAsync(request),
                GrowthTrend = entityStats.GrowthRate > 0 ? "up" :
                             entityStats.GrowthRate < 0 ? "down" : "stable"
            };

            return ServiceResult.Ok(dto);
        }

        private async Task<int> GetPreviousPeriodVisitsAsync(GetGrowthStatsQuery request)
        {
            if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                return 0;

            var periodDays = (request.ToDate.Value - request.FromDate.Value).Days;
            var previousFromDate = request.FromDate.Value.AddDays(-periodDays);
            var previousToDate = request.FromDate.Value.AddDays(-1);

            var previousStats = await _visitTrackingService.GetEntityStatsAsync(
                request.EntityType, request.EntityId, previousFromDate, previousToDate);

            return previousStats.TotalVisits;
        }
    }
}
