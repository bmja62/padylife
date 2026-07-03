using Entities.Chats;

namespace Services.Hubs.DTOs
{
    public class SendMessageDTO
    {
        public long RoomId { get; set; }
        public string EncryptedContent { get; set; }
        public MessageType Type { get; set; }
        public long? ReplyToMessageId { get; set; }
        public string EncryptionMetadata { get; set; }
    }
    public class ChatMessageViewModel
    {
        public long MessageId { get; set; }
        public long RoomId { get; set; }
        public long SenderId { get; set; }
        public string EncryptedContent { get; set; }
        public MessageType Type { get; set; }
        public long? ReplyToMessageId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string SenderName { get; set; }
        public List<ReactionType>? Reactions { get; set; }
        public ChatMessageStatus Status { get; internal set; }
    }

    public class ChatInfoViewModel
    {
        public long ChatId { get; internal set; }
        public string ChatRoomName { get; internal set; }
        public string ChatRoomUserPlanTitle { get; internal set; }
        public bool IsGroupChatRoom { get; internal set; }
        public bool IsExpert { get; internal set; }
    }
    public class ChatUserInfoViewModel
    {
        public long UserId { get; internal set; }
        public string UserFullName { get; internal set; }
        public bool IsOnline { get; internal set; }
        public string ProfileImage { get; internal set; }
    }
    public class LoadUserChatsViewModel
    {
        public ChatInfoViewModel Chat { get; internal set; }
        public ChatUserInfoViewModel UserInfo { get; internal set; }
        public int? MessageCount { get; internal set; }
        public int UnReadMessageCount { get; internal set; }
        public string LastMessage { get; internal set; }
        public DateTime? LastMessageTime { get; internal set; }
    }
}
