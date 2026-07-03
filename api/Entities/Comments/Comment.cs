using Entities.Common;
using Entities.Users;

namespace Entities.Comments
{
    public class Comment : BaseEntity<long>
    {
        //FKs
        /// <summary>
        /// اتصال به Entity
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// آیدی کاربر
        /// </summary>
        public long CreatedByUserId { get; set; }

        /// <summary>
        /// ریپلای به نظر دیگر (nullable = یعنی ممکنه یک کامنت اصلی باشه)
        /// </summary>
        public long? ParentCommentId { get; set; }

        //Props

        /// <summary>
        /// موجودیت های این سایت
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// متن نظر
        /// </summary>
        public string Text { get; set; }

        public bool IsApproved { get; private set; } = false; // تایید شده توسط ادمین
        public DateTime? ApprovedAt { get; private set; }  // تاریخ تایید

        //Navigations
        public Comment? ParentComment { get; set; }

        /// <summary>
        /// پاسخ‌ها به این کامنت
        /// </summary>
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        /// <summary>
        /// ساخته شده توسط شخص
        /// </summary>
        public User CreatedByUser { get; set; }

        /// <summary>
        /// لایک و دیسلایک ها
        /// </summary>
        public ICollection<CommentReaction> Reactions { get; private set; } = new List<CommentReaction>();

        private Comment() { } // EF Core

        public Comment(long entityId, EntityType entityType, long userId, string text, long? parentCommentId = null)
        {
            EntityId = entityId;
            EntityType = entityType;
            CreatedByUserId = userId;
            Text = text;
            ParentCommentId = parentCommentId;
        }

        public Comment Reply(long userId, string replyText)
        {
            var reply = new Comment(EntityId, EntityType, userId, replyText, Id);
            Replies.Add(reply);
            return reply;
        }

        public int LikeCount => Reactions?.Count(r => r.ReactionType == ReactionType.Like) ?? 0;
        public int DislikeCount => Reactions?.Count(r => r.ReactionType == ReactionType.Dislike) ?? 0;

        public void React(long userId, ReactionType reactionType)
        {
            var existing = Reactions.FirstOrDefault(r => r.UserId == userId);
            if (existing != null)
            {
                existing.SetReaction(reactionType);
            }
            else
            {
                Reactions.Add(new CommentReaction(Id, userId, reactionType));
            }
        }
        public void EditComment(string newText)
        {
            Text = newText;
        }

        public void ApproveComment()
        {
            IsApproved = true;
            ApprovedAt = DateTime.UtcNow;
        }
    }
}
