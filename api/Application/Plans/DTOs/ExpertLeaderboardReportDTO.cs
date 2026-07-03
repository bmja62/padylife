namespace Application.Plans.DTOs
{
    public class ExpertLeaderboardReportDTO
    {
        public List<EpertLeaderboardUserDTO> Leaders { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class EpertLeaderboardUserDTO
    {
        public int Rank { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public double AvgRate { get; set; }
    }
}
