using Entities.Common;

namespace Entities.Users
{
    public class UserBadge : BaseEntity<long>
    {
        public long UserId { get; set; }
        public long SimilarUserId { get; set; }
        public SimilarityBadgeType BadgeType { get; set; }
        public double SimilarityPercent { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public User SimilarUser { get; set; }

        public static SimilarityBadgeType? GetBadgeType(double similarity)
        {
            if (similarity >= 95) return SimilarityBadgeType.TwinFriend;
            if (similarity >= 85) return SimilarityBadgeType.GoldenThinker;
            if (similarity >= 70) return SimilarityBadgeType.SilverWalker;
            if (similarity >= 60) return SimilarityBadgeType.FriendlyMate;
            if (similarity <= 60) return SimilarityBadgeType.TiredFriend;
            return null;
        }
        public static (string title, string icon) GetBadgeMeta(SimilarityBadgeType? type) =>
        type switch
        {
            SimilarityBadgeType.TwinFriend => ("دوست دوقلوی من", "👯‍♂️"),
            SimilarityBadgeType.GoldenThinker => ("هم‌فکر طلایی", "🧠🏅"),
            SimilarityBadgeType.SilverWalker => ("هم‌مسیر نقره‌ای", "🚶‍♂️✨"),
            SimilarityBadgeType.FriendlyMate => ("همراه آشنا", "👋"),
            SimilarityBadgeType.TeamChampion => ("تیم برتر ماه", "🏆"),
            SimilarityBadgeType.TiredFriend => ("یار خسته من", "😴"),
            _ => ("", "")
        };
    }

    public enum SimilarityBadgeType
    {
        TwinFriend = 1,     // 👯‍♂️
        GoldenThinker = 2,  // 🧠🏅
        SilverWalker = 3,   // 🚶‍♂️✨
        FriendlyMate = 4,    // 👋
        TeamChampion = 5, // 🏆 تیم برتر ماه
        TiredFriend = 6 // 😴
    }
}
