using Entities.Common;
using Entities.Users;

namespace Entities.Chats
{
    public class MessageStatus : BaseEntity<long>
    {
        public long MessageId { get; private set; }
        public long ReceiverId { get; private set; }

        public ChatMessageStatus Status { get; private set; }
        public ChatMessage Message { get; private set; }
        public User Receiver { get; private set; }

        public void UpdateStatus(ChatMessageStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        private MessageStatus() { }

        public MessageStatus(long messageId, long receiverId, ChatMessageStatus status)
        {
            MessageId = messageId;
            ReceiverId = receiverId;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
