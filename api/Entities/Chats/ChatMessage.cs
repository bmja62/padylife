using Entities.Common;
using Entities.Users;

namespace Entities.Chats
{
    public class ChatMessage : BaseEntity<long>
    {
        private ChatMessage() { }

        public ChatMessage(long senderId, long roomId, string encryptedContent, long? replyToMessageId, MessageType type, string? encryptionMetadata = null)
        {
            SenderId = senderId;
            ChatRoomId = roomId;
            EncryptedContent = encryptedContent;
            ReplyToMessageId = replyToMessageId;
            Type = type;
            Status = ChatMessageStatus.Sent;
            CreatedAt = DateTime.UtcNow;
            EncryptionMetadata = encryptionMetadata;
        }

        public long SenderId { get; private set; }
        public long ChatRoomId { get; private set; }
        /// <summary>
        /// محتوای رمزنگاری‌شده (Base64 یا Hex)
        /// </summary>
        public string EncryptedContent { get; private set; }
        public MessageType Type { get; private set; }
        public ChatMessageStatus Status { get; private set; }

        public long? ReplyToMessageId { get; private set; }

        // اگر بخواهی رمزنگاری در سمت سرور هم باشد (نه فقط E2E)
        public string? EncryptionMetadata { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public virtual User Sender { get; private set; }
        public virtual ChatRoom ChatRoom { get; private set; }
        public ICollection<MessageStatus> Statuses { get; set; }

        public ICollection<ChatMessageReaction> Reactions { get; private set; } = new List<ChatMessageReaction>();

        // Methods
        public void MarkAsDelivered() => Status = ChatMessageStatus.Delivered;
        public void MarkAsRead() => Status = ChatMessageStatus.Read;
        public void MarkAsReceived() => Status = ChatMessageStatus.Received;
        public void AddReaction(ChatMessageReaction chatMessageReaction) => Reactions.Add(chatMessageReaction);
        public ChatMessage(long senderId, long roomId, string content, long? replyToMessageId, MessageType type)
        {
            SenderId = senderId;
            ChatRoomId = roomId;
            EncryptedContent = content;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            ReplyToMessageId = replyToMessageId;
        }

        public ChatMessage ForwardTo(long roomId, long forwarderId)
        {
            return new ChatMessage(
                senderId: forwarderId,
                roomId: roomId,
                content: this.EncryptedContent,
                replyToMessageId: this.Id,
                type: this.Type
            );
        }
    }

}
