using Application.Cqrs.Queris;
using Services;

namespace Application.Visits.Queries
{
    // Application/Visits/Queries/GetSectionStatsQuery.cs
    public class GetSectionStatsQuery : IQuery<ServiceResult<List<SectionStatsDTO>>>
    {
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GetSectionStatsQuery(DateTime? fromDate, DateTime? toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }


}
