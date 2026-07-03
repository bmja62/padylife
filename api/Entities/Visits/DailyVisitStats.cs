using Entities.Common;

namespace Entities.Visits
{
    public class DailyVisitStats : BaseEntity<long>
    {
        public DateTime StatDate { get; set; }
        public string PageUrl { get; set; }
        public string DetectedEntityType { get; set; }
        public long? DetectedEntityId { get; set; }
        public string DetectedSection { get; set; }
        public int UniqueVisits { get; set; }
        public int TotalVisits { get; set; }
    }
}
