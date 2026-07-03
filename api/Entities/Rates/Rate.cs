using Entities.Common;
using Entities.Users;

namespace Entities.Rates
{
    public class Rate : BaseEntity<long>
    {

        private Rate() { } // EF Core

        public Rate(long entityId, EntityType entityType, int ratingValue, long userId)
        {
            EntityId = entityId;
            EntityType = entityType;
            RatingValue = ratingValue;
            CreatedByUserId = userId;
        }


        //FKs

        /// <summary>
        /// شناسه موجودیت (مثل ProductId یا PostId)
        /// </summary>
        public long EntityId { get; set; }
        /// <summary>
        /// شناسه کاربر (یا می‌توانید از User ارتباط بگیرید)
        /// </summary>
        public long CreatedByUserId { get; set; }


        //Props

        /// <summary>
        /// نوع موجودیت (Product, Post, ...)
        /// </summary>
        public EntityType EntityType { get; set; }
        /// <summary>
        /// امتیاز (مثلاً 1 تا 5)
        /// </summary>
        public int RatingValue { get; set; }

        //Navigations
        public User CreatedByUser { get; set; }


        //Methods
        public static double CalculateAverage(IEnumerable<Rate> ratings) =>
            ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;

        public static bool HasUserRated(IEnumerable<Rate> ratings, long userId) =>
            ratings.Any(r => r.CreatedByUserId == userId);
    }
}
