namespace Services.Services.OnlineUsersServices
{
    public class OnlineUserStatsDto
    {
        public int TotalOnline { get; set; }
        public int TotalActive { get; set; }
        public List<PageStatDto> Pages { get; set; } = new();
        public List<LocationStatDto> Countries { get; set; } = new();
        public DateTime RecordedAt { get; set; }
    }
}