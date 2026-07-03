using Entities.Common;
using Entities.Users;

namespace Entities.Chats
{
    public class ChatRoomParticipant : BaseEntity<long>
    {
        private ChatRoomParticipant() { }

        public ChatRoomParticipant(long roomId, long userId)
        {
            ChatRoomId = roomId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
        }

        public long ChatRoomId { get; private set; }
        public long UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        public virtual ChatRoom ChatRoom { get; private set; }
        public virtual User User { get; private set; }

        public static ChatRoomParticipant Create(long userId)
        {
            return new ChatRoomParticipant
            {
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };
        }
    }

}
