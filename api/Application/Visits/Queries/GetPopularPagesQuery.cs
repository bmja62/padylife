using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visits.Queries
{
    public class GetPopularPagesQuery : IQuery<ServiceResult<List<PopularPageDTO>>>
    {
        public int Top { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetPopularPagesQuery(int top = 10, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Top = top > 0 && top <= 100 ? top : 10; // محدودیت برای جلوگیری از بار زیاد
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetPopularPagesQueryHandler
        : IQueryHandler<GetPopularPagesQuery, ServiceResult<List<PopularPageDTO>>>
    {
        private readonly IRepository<DailyVisitStats> _dailyStatsRepository;

        public GetPopularPagesQueryHandler(IRepository<DailyVisitStats> dailyStatsRepository)
        {
            _dailyStatsRepository = dailyStatsRepository;
        }

        public async Task<ServiceResult<List<PopularPageDTO>>> Handle(
            GetPopularPagesQuery request, CancellationToken cancellationToken)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            if (request.FromDate.HasValue)
                query = query.Where(s => s.StatDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(s => s.StatDate <= request.ToDate.Value);

            var popularPages = await query
                .GroupBy(s => new { s.PageUrl, s.DetectedEntityType, s.DetectedEntityId })
                .Select(g => new PopularPageDTO
                {
                    PageUrl = g.Key.PageUrl,
                    EntityType = g.Key.DetectedEntityType,
                    EntityId = g.Key.DetectedEntityId,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderByDescending(p => p.TotalVisits)
                .ThenByDescending(p => p.UniqueVisits)
                .Take(request.Top)
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(popularPages);
        }
    }
}
