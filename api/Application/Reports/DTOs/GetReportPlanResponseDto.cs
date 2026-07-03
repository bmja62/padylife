namespace Application.Reports.DTOs
{
    public class GetReportPlanResponseDto
    {
        public long PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanImageUrl { get; set; }
        public int PersonCount { get; set; }
        public double AverageAge { get; set; }
        public double PercentageOfPeopleWhoCompletedThePlan { get; set; }
        public double ManGender { get; set; }
        public double WomanGender { get; set; }
    }

}
