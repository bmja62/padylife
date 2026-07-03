using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Visits;
using Services;
using Services.Services.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetPageStatsQuery.cs
    public class GetPageStatsQuery : IQuery<ServiceResult<PageVisitStatsDTO>>
    {
        public string PageUrl { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetPageStatsQuery(string pageUrl, DateTime? fromDate, DateTime? toDate)
        {
            PageUrl = pageUrl;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetPageStatsQueryHandler
       : IQueryHandler<GetPageStatsQuery, ServiceResult<PageVisitStatsDTO>>
    {
        private readonly IRepository<DailyVisitStats> _dailyStatsRepository;
        private readonly IRepository<PageVisit> _pageVisitRepository;
        private readonly IVisitTrackingService _visitTrackingService;

        public GetPageStatsQueryHandler(
            IRepository<DailyVisitStats> dailyStatsRepository,
            IRepository<PageVisit> pageVisitRepository,
            IVisitTrackingService visitTrackingService)
        {
            _dailyStatsRepository = dailyStatsRepository;
            _pageVisitRepository = pageVisitRepository;
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult<PageVisitStatsDTO>> Handle(
            GetPageStatsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PageUrl))
                return ServiceResult.BadRequest<PageVisitStatsDTO>("آدرس صفحه الزامی است");

            var pageStats = await _visitTrackingService.GetPageStatsAsync(
                request.PageUrl, request.FromDate, request.ToDate);

            var dto = new PageVisitStatsDTO
            {
                PageUrl = pageStats.PageUrl,
                TotalVisits = pageStats.TotalVisits,
                UniqueVisits = pageStats.UniqueVisits,
                BounceRate = pageStats.BounceRate,
                AvgTimeOnPage = pageStats.AvgTimeOnPage,
                VisitSources = pageStats.VisitSources.Select(s => new VisitSourceDTO
                {
                    Referrer = s.Referrer,
                    Visits = s.Visits,
                    Percentage = s.Percentage
                }).ToList()
            };

            return ServiceResult.Ok(dto);
        }
    }
}
