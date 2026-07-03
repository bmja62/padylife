using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.Visits;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetRealtimeStatsQuery.cs
    public class GetRealtimeStatsQuery : IQuery<ServiceResult<RealtimeStatsDTO>>
    {
        public int Hours { get; }

        public GetRealtimeStatsQuery(int hours)
        {
            Hours = hours;
        }
    }

    public class GetSectionStatsQueryHandler
        : IQueryHandler<GetSectionStatsQuery, ServiceResult<List<SectionStatsDTO>>>
    {
        private readonly IVisitTrackingService _visitTrackingService;

        public GetSectionStatsQueryHandler(IVisitTrackingService visitTrackingService)
        {
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult<List<SectionStatsDTO>>> Handle(
            GetSectionStatsQuery request, CancellationToken cancellationToken)
        {
            var sectionStats = await _visitTrackingService.GetSectionStatsAsync(
                request.FromDate, request.ToDate);

            var dtos = sectionStats.Select(s => new SectionStatsDTO
            {
                Section = s.Section,
                TotalVisits = s.TotalVisits,
                UniqueVisits = s.UniqueVisits,
                Percentage = s.Percentage
            }).ToList();

            return ServiceResult.Ok(dtos);
        }
    }

    public class GetRealtimeStatsQueryHandler
        : IQueryHandler<GetRealtimeStatsQuery, ServiceResult<RealtimeStatsDTO>>
    {
        private readonly IRepository<PageVisit> _pageVisitRepository;

        public GetRealtimeStatsQueryHandler(IRepository<PageVisit> pageVisitRepository)
        {
            _pageVisitRepository = pageVisitRepository;
        }

        public async Task<ServiceResult<RealtimeStatsDTO>> Handle(
            GetRealtimeStatsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var oneHourAgo = now.AddHours(-1);
            var todayStart = now.Date;
            var hoursAgo = now.AddHours(-request.Hours);

            // بازدیدکنندگان فعال (در 30 دقیقه گذشته)
            var activeVisitorsThreshold = now.AddMinutes(-30);
            var activeVisitors = await _pageVisitRepository.TableNoTracking
                .Where(v => v.VisitDate >= activeVisitorsThreshold)
                .Select(v => v.SessionId)
                .Distinct()
                .CountAsync(cancellationToken);

            // بازدیدهای یک ساعت گذشته
            var visitsLastHour = await _pageVisitRepository.TableNoTracking
                .CountAsync(v => v.VisitDate >= oneHourAgo, cancellationToken);

            // بازدیدهای امروز
            var visitsToday = await _pageVisitRepository.TableNoTracking
                .CountAsync(v => v.VisitDate >= todayStart, cancellationToken);

            // صفحات فعال
            var activePages = await _pageVisitRepository.TableNoTracking
                .Where(v => v.VisitDate >= activeVisitorsThreshold)
                .GroupBy(v => v.PageUrl)
                .Select(g => new ActivePageDTO
                {
                    PageUrl = g.Key,
                    ActiveVisitors = g.Select(v => v.SessionId).Distinct().Count()
                })
                .OrderByDescending(p => p.ActiveVisitors)
                .Take(10)
                .ToListAsync(cancellationToken);

            var realtimeStats = new RealtimeStatsDTO
            {
                ActiveVisitors = activeVisitors,
                VisitsLastHour = visitsLastHour,
                VisitsToday = visitsToday,
                ActivePages = activePages
            };

            return ServiceResult.Ok(realtimeStats);
        }
    }
}
