namespace Application.Users.DTOs
{
    public class MyBadgeDTO
    {
        public string BadgeTitle { get; set; }
        public string BadgeIcon { get; set; }
        public double SimilarityPercent { get; set; }
        public string SimilarUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
