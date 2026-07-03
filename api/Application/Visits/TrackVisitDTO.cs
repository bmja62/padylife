using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visits
{
    public class TrackVisitDTO
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public string Referrer { get; set; }
    }

    public class VisitStatsDTO
    {
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public string PageUrl { get; set; }
        public string Section { get; set; }
        public int UniqueVisits { get; set; }
        public int TotalVisits { get; set; }
        public DateTime StatDate { get; set; }
    }

    public class VisitFilterDTO
    {
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public string Section { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PageUrlPattern { get; set; }
    }

    public class PopularPageDTO
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
    }

    public class ProcessDailyStatsDTO
    {
        public DateTime TargetDate { get; set; }
    }

    public class PageVisitStatsDTO
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public decimal BounceRate { get; set; }
        public decimal AvgTimeOnPage { get; set; }
        public List<VisitSourceDTO> VisitSources { get; set; } = new();
    }

    public class SectionStatsDTO
    {
        public string Section { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DailyChartDataDTO
    {
        public DateTime Date { get; set; }
        public int Visits { get; set; }
        public int UniqueVisits { get; set; }
    }

    public class VisitSummaryDTO
    {
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public int TotalPages { get; set; }
        public decimal AvgVisitsPerDay { get; set; }
        public PopularPageDTO MostPopularPage { get; set; }
        public string MostActiveSection { get; set; }
        public List<EntityStatsDTO> TopEntities { get; set; } = new();
    }

    public class ProcessDateRangeStatsDTO
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class CleanupDataDTO
    {
        public int KeepDays { get; set; } = 365;
    }

    public class GrowthStatsDTO
    {
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public decimal GrowthRate { get; set; }
        public int CurrentPeriodVisits { get; set; }
        public int PreviousPeriodVisits { get; set; }
        public string GrowthTrend { get; set; } // "up", "down", "stable"
    }

    public class VisitSourceDTO
    {
        public string Referrer { get; set; }
        public int Visits { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RealtimeStatsDTO
    {
        public int ActiveVisitors { get; set; }
        public int VisitsLastHour { get; set; }
        public int VisitsToday { get; set; }
        public List<ActivePageDTO> ActivePages { get; set; } = new();
    }

    public class ActivePageDTO
    {
        public string PageUrl { get; set; }
        public int ActiveVisitors { get; set; }
    }

    public class EntityStatsDTO
    {
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
    }
}
