using Entities.Common;
using Entities.Users;

namespace Entities.Comments
{
    public class CommentReaction : BaseEntity<long>
    {
        //FKs
        public long CommentId { get; private set; }
        public long UserId { get; private set; }

        //Props
        public ReactionType ReactionType { get; private set; }

        //Navigations
        public Comment Comment { get; private set; }
        public User User { get; set; }

        private CommentReaction() { } // EF Core

        public CommentReaction(long commentId, long userId, ReactionType reactionType)
        {
            CommentId = commentId;
            UserId = userId;
            ReactionType = reactionType;
        }

        public void SetReaction(ReactionType newType)
        {
            ReactionType = newType;
        }
    }
}
