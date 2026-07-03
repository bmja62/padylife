namespace Application.Plans.DTOs
{
    public class PlanLeaderboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalQuestions { get; set; }

        public List<PlanLeaderboardUserStatsDTO> TopUsers { get; set; }
        public PlanLeaderboardUserStatsDTO CurrentUser { get; set; }
    }

    public class PlanLeaderboardUserStatsDTO
    {
        public long UserId { get; set; }
        public string FullName { get; set; }

        public int AnswerCount { get; set; }
        public int TotalPoints { get; set; }
        public int Rank { get; set; }

        public double ParticipationPercent { get; set; } // AnswerCount / TotalQuestions * 100
    }

}
