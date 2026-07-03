using Common.GridResults;
using Entities.DailyFeelings;

namespace Application.DailyFeelings.DTOs
{
    public class GetAllDailyFeelingQueryDTO : GlobalGrid
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DailyFeelingsType? Type { get; set; }
    }
}
