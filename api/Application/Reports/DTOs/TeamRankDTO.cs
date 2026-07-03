namespace Application.Reports.DTOs
{
    public class TeamRankDTO
    {
        public string TeamName { get; set; }
        public int TotalPoints { get; set; }
        public int TotalAnswers { get; set; }
        public int Rank { get; set; }
        public List<long> MemberUserIds { get; set; }
        public bool IsTopTeam { get; set; } // برای اعطای نشان
    }
}
