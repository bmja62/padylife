namespace Application.Plans.DTOs
{
    public class UserPlanLeaderboardOverviewDTO
    {
        public long PlanId { get; set; }
        public string PlanTitle { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalUsers { get; set; }

        public List<PlanLeaderboardUserStatsDTO> TopUsers { get; set; }
        public PlanLeaderboardUserStatsDTO CurrentUser { get; set; }
    }
}
