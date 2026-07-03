using Entities.Common;
using Entities.Users;

namespace Entities.Chats
{
    public class ChatMessageReaction : BaseEntity<long>
    {
        private ChatMessageReaction()
        {

        }
        public ChatMessageReaction(long messageId, long userId, ReactionType reactionType)
        {
            ChatMessageId = messageId;
            UserId = userId;
            ReactionType = reactionType;
            CreatedAt = DateTime.UtcNow;
        }

        public long ChatMessageId { get; private set; }
        public long UserId { get; private set; }
        public ReactionType ReactionType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual ChatMessage Message { get; private set; }
        public virtual User User { get; private set; }

        public void UpdateReaction(ReactionType reactionType) => ReactionType = reactionType;
    }

}
