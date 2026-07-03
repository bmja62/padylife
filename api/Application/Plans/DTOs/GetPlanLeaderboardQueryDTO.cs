namespace Application.Plans.DTOs
{
    public class GetPlanLeaderboardQueryDTO
    {
        public long PlanId { get; set; }
        public int TopCount { get; set; } = 10;
    }
    public class PlanLeaderboardUserDTO
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int AnswerCount { get; set; }
        public int TotalPoints { get; set; }
        public int Rank { get; set; }
        public string ProfileImage { get; internal set; }
    }
}
