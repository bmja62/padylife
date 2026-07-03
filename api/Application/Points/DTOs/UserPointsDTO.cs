namespace Application.Points.DTOs
{
    public class UserPointsDTO
    {
        public long UserId { get; set; }
        public int AvailablePoints { get; set; }
        public int EarnedPoints { get; set; }
        public int ConsumedPoints { get; set; }
        public int PointsToMoneyRatio { get; set; }
        public decimal MoneyValue { get; set; }
    }
}
