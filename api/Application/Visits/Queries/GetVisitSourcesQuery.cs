using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetVisitSourcesQuery.cs
    public class GetVisitSourcesQuery : IQuery<ServiceResult<List<VisitSourceDTO>>>
    {
        public string PageUrl { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetVisitSourcesQuery(string pageUrl, DateTime? fromDate, DateTime? toDate)
        {
            PageUrl = pageUrl;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetVisitSourcesQueryHandler
       : IQueryHandler<GetVisitSourcesQuery, ServiceResult<List<VisitSourceDTO>>>
    {
        private readonly IRepository<PageVisit> _pageVisitRepository;

        public GetVisitSourcesQueryHandler(IRepository<PageVisit> pageVisitRepository)
        {
            _pageVisitRepository = pageVisitRepository;
        }

        public async Task<ServiceResult<List<VisitSourceDTO>>> Handle(
            GetVisitSourcesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PageUrl))
                return ServiceResult.BadRequest<List<VisitSourceDTO>>("آدرس صفحه الزامی است");

            var query = _pageVisitRepository.TableNoTracking
                .Where(v => v.PageUrl == request.PageUrl && !string.IsNullOrEmpty(v.Referrer));

            if (request.FromDate.HasValue)
                query = query.Where(v => v.VisitDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(v => v.VisitDate <= request.ToDate.Value);

            var totalVisits = await query.CountAsync(cancellationToken);

            var sources = await query
                .GroupBy(v => v.Referrer)
                .Select(g => new VisitSourceDTO
                {
                    Referrer = g.Key,
                    Visits = g.Count()
                })
                .OrderByDescending(s => s.Visits)
                .Take(10)
                .ToListAsync(cancellationToken);

            // محاسبه درصد
            foreach (var source in sources)
            {
                source.Percentage = totalVisits > 0 ? (decimal)source.Visits / totalVisits * 100 : 0;
            }

            return ServiceResult.Ok(sources);
        }
    }
}
