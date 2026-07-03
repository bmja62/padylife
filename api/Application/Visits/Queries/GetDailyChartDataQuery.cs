using Application.Cqrs.Queris;
using Services;
using Services.Services.Visits;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetDailyChartDataQuery.cs
    public class GetDailyChartDataQuery : IQuery<ServiceResult<List<DailyChartDataDTO>>>
    {
        public string EntityType { get; }
        public long? EntityId { get; }
        public int Days { get; }

        public GetDailyChartDataQuery(string entityType, long? entityId, int days)
        {
            EntityType = entityType;
            EntityId = entityId;
            Days = days;
        }
    }

    public class GetDailyChartDataQueryHandler
        : IQueryHandler<GetDailyChartDataQuery, ServiceResult<List<DailyChartDataDTO>>>
    {
        private readonly IVisitTrackingService _visitTrackingService;

        public GetDailyChartDataQueryHandler(IVisitTrackingService visitTrackingService)
        {
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult<List<DailyChartDataDTO>>> Handle(
            GetDailyChartDataQuery request, CancellationToken cancellationToken)
        {
            var chartData = await _visitTrackingService.GetDailyChartDataAsync(
                request.EntityType, request.EntityId, request.Days);

            var dtos = chartData.Select(d => new DailyChartDataDTO
            {
                Date = d.Date,
                Visits = d.Visits,
                UniqueVisits = d.UniqueVisits
            }).ToList();

            return ServiceResult.Ok(dtos);
        }
    }
}
