using Common;
using Data.Contracts;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.ChatServices
{
    public interface IChatService
    {
        Task<ChatMessage> SendMessageAsync(long senderId, long roomId, string encryptedContent, MessageType type, long? replyToMessageId = null);
        Task UpdateMessageStatusAsync(long messageId, long userId, ChatMessageStatus newStatus);
    }
    public class ChatService : IChatService, IScopedDependency
    {
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<ChatRoom> _chatRoomRepository;
        private readonly IRepository<MessageStatus> _messageStatusRepository;

        public ChatService(
            IRepository<ChatMessage> chatMessageRepository,
            IRepository<ChatRoom> chatRoomRepository,
            IRepository<MessageStatus> messageStatusRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatRoomRepository = chatRoomRepository;
            _messageStatusRepository = messageStatusRepository;
        }

        public async Task<ChatMessage> SendMessageAsync(long senderId, long roomId, string encryptedContent, MessageType type, long? replyToMessageId = null)
        {
            var chatRoom = await _chatRoomRepository.Table
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (chatRoom == null)
                throw new Exception("Chat room not found.");

            var message = new ChatMessage(senderId, roomId, encryptedContent, replyToMessageId, type);
            await _chatMessageRepository.AddAsync(message, CancellationToken.None);

            foreach (var participant in chatRoom.Participants)
            {
                var status = new MessageStatus(message.Id, participant.UserId,
                    participant.UserId == senderId ? ChatMessageStatus.Sent : ChatMessageStatus.Sent);

                await _messageStatusRepository.AddAsync(status, CancellationToken.None);
            }

            return message;
        }

        public async Task UpdateMessageStatusAsync(long messageId, long userId, ChatMessageStatus newStatus)
        {
            var status = await _messageStatusRepository.Table
                .FirstOrDefaultAsync(s => s.MessageId == messageId && s.ReceiverId == userId);

            if (status == null)
                return;

            if ((int)newStatus > (int)status.Status)
            {
                status.UpdateStatus(newStatus);
                await _messageStatusRepository.UpdateAsync(status, CancellationToken.None);
            }
        }
    }


}
