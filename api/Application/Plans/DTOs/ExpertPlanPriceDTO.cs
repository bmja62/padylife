namespace Application.Plans.DTOs
{
    public class ExpertPlanPriceDTO
    {
        public long ExpertId { get; set; }
        public string ExpertFullName { get; set; }
        public long PlanId { get; set; }
        public string PlanTitle { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public long Id { get; internal set; }
    }

    public class ExpertPlanPriceForUIDTO : ExpertPlanPriceDTO
    {
        public string JobTitle { get; set; }
        public double? AverageRate { get; set; }
        public int? CompanionsCount { get; set; }

    }
}
