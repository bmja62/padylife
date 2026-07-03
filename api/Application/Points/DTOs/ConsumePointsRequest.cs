using Entities.Common;

namespace Application.Points.DTOs
{
    public class ConsumePointsRequest
    {
        public long UserId { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
        public long? ReferenceId { get; set; }
        public EntityType ReferenceType { get; set; }
    }
}
