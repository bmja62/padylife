using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Visits.DTOs
{
    public class EntityVisitStats
    {
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public string EntityName { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public decimal GrowthRate { get; set; } // نرخ رشد نسبت به دوره قبل
        public List<DailyVisitData> DailyData { get; set; } = new();
    }

    public class PageVisitStats
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public decimal BounceRate { get; set; }
        public decimal AvgTimeOnPage { get; set; }
        public List<VisitSource> VisitSources { get; set; } = new();
    }

    public class PopularPage
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public string EntityType { get; set; }
        public long? EntityId { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public int Rank { get; set; }
    }

    public class SectionStats
    {
        public string Section { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DailyChartData
    {
        public DateTime Date { get; set; }
        public int Visits { get; set; }
        public int UniqueVisits { get; set; }
    }

    public class VisitSource
    {
        public string Referrer { get; set; }
        public int Visits { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DailyVisitData
    {
        public DateTime Date { get; set; }
        public int Visits { get; set; }
        public int UniqueVisits { get; set; }
    }

    public class VisitSummary
    {
        public int TotalVisits { get; set; }
        public int UniqueVisits { get; set; }
        public int TotalPages { get; set; }
        public decimal AvgVisitsPerDay { get; set; }
        public PopularPage MostPopularPage { get; set; }
        public string MostActiveSection { get; set; }
        public List<EntityVisitStats> TopEntities { get; set; } = new();
    }
}
