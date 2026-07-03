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
    public class GetVisitStatsQuery : IQuery<ServiceResult<List<VisitStatsDTO>>>
    {
        public VisitFilterDTO Filter { get; }

        public GetVisitStatsQuery(VisitFilterDTO filter)
        {
            Filter = filter;
        }
    }

    public class GetVisitStatsQueryHandler
        : IQueryHandler<GetVisitStatsQuery, ServiceResult<List<VisitStatsDTO>>>
    {
        private readonly IRepository<DailyVisitStats> _dailyStatsRepository;

        public GetVisitStatsQueryHandler(IRepository<DailyVisitStats> dailyStatsRepository)
        {
            _dailyStatsRepository = dailyStatsRepository;
        }

        public async Task<ServiceResult<List<VisitStatsDTO>>> Handle(
            GetVisitStatsQuery request, CancellationToken cancellationToken)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            // اعمال فیلترها
            if (!string.IsNullOrEmpty(request.Filter.EntityType))
                query = query.Where(s => s.DetectedEntityType == request.Filter.EntityType);

            if (request.Filter.EntityId.HasValue)
                query = query.Where(s => s.DetectedEntityId == request.Filter.EntityId);

            if (!string.IsNullOrEmpty(request.Filter.Section))
                query = query.Where(s => s.DetectedSection == request.Filter.Section);

            if (!string.IsNullOrEmpty(request.Filter.PageUrlPattern))
                query = query.Where(s => s.PageUrl.Contains(request.Filter.PageUrlPattern));

            if (request.Filter.FromDate.HasValue)
                query = query.Where(s => s.StatDate >= request.Filter.FromDate.Value);

            if (request.Filter.ToDate.HasValue)
                query = query.Where(s => s.StatDate <= request.Filter.ToDate.Value);

            var stats = await query
                .OrderByDescending(s => s.StatDate)
                .ThenBy(s => s.DetectedEntityType)
                .Select(s => new VisitStatsDTO
                {
                    EntityType = s.DetectedEntityType,
                    EntityId = s.DetectedEntityId,
                    PageUrl = s.PageUrl,
                    Section = s.DetectedSection,
                    UniqueVisits = s.UniqueVisits,
                    TotalVisits = s.TotalVisits,
                    StatDate = s.StatDate
                })
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(stats);
        }
    }
}
