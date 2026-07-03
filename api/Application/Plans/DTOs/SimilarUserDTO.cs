namespace Application.Plans.DTOs
{
    public class SimilarUserDTO
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int SharedAnswerCount { get; set; }
        public double SimilarityPercent { get; set; } // مثلاً 84.6 درصد
        public string BadgeTitle { get; internal set; }
        public string BadgeIcon { get; internal set; }
    }
}
