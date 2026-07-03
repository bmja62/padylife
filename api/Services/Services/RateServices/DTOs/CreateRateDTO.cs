using Entities.Common;

namespace Services.Services.RateServices.DTOs
{
    public class RateDTO
    {
        public long EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public int RatingValue { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateRateDTO
    {
        public int RatingValue { get; set; }  // 1..5
    }
}
