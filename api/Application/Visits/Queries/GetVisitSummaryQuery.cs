using Application.Cqrs.Queris;
using Services;
using Services.Services.Visits;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetVisitSummaryQuery.cs
    public class GetVisitSummaryQuery : IQuery<ServiceResult<VisitSummaryDTO>>
    {
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetVisitSummaryQuery(DateTime? fromDate, DateTime? toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetVisitSummaryQueryHandler
       : IQueryHandler<GetVisitSummaryQuery, ServiceResult<VisitSummaryDTO>>
    {
        private readonly IVisitTrackingService _visitTrackingService;

        public GetVisitSummaryQueryHandler(IVisitTrackingService visitTrackingService)
        {
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult<VisitSummaryDTO>> Handle(
            GetVisitSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = await _visitTrackingService.GetVisitSummaryAsync(
                request.FromDate, request.ToDate);

            var dto = new VisitSummaryDTO
            {
                TotalVisits = summary.TotalVisits,
                UniqueVisits = summary.UniqueVisits,
                TotalPages = summary.TotalPages,
                AvgVisitsPerDay = summary.AvgVisitsPerDay,
                MostActiveSection = summary.MostActiveSection,
                MostPopularPage = summary.MostPopularPage != null ? new PopularPageDTO
                {
                    PageUrl = summary.MostPopularPage.PageUrl,
                    EntityType = summary.MostPopularPage.EntityType,
                    EntityId = summary.MostPopularPage.EntityId,
                    TotalVisits = summary.MostPopularPage.TotalVisits,
                    UniqueVisits = summary.MostPopularPage.UniqueVisits
                } : null,
                TopEntities = summary.TopEntities.Select(e => new EntityStatsDTO
                {
                    EntityType = e.EntityType,
                    EntityId = e.EntityId,
                    TotalVisits = e.TotalVisits,
                    UniqueVisits = e.UniqueVisits
                }).ToList()
            };

            return ServiceResult.Ok(dto);
        }
    }
}
