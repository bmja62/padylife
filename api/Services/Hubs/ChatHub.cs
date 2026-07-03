using Common.GridResults;
using Data.Contracts;
using Entities.Chats;
using Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Hubs.DTOs;
using Services.Services.ChatServices;
using Services.Services.UserConnectionServices;

namespace Services.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<ChatRoom> _chatRoomRepository;
        private readonly IRepository<MessageStatus> _messageStatusRepository;
        private readonly IRepository<ChatRoomParticipant> _chatRoomParticipantRepository;
        private readonly IRepository<Expert> _expertRepository;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IChatService _chatService;

        public ChatHub(IRepository<ChatMessage> chatMessageRepository, IUserConnectionManager userConnectionManager, IRepository<ChatRoom> chatRoomRepository, IRepository<MessageStatus> messageStatusRepository, IRepository<ChatRoomParticipant> chatRoomParticipantRepository, IRepository<Expert> expertRepository, IChatService chatService)
        {
            _chatMessageRepository = chatMessageRepository;
            _userConnectionManager = userConnectionManager;
            _chatRoomRepository = chatRoomRepository;
            _messageStatusRepository = messageStatusRepository;
            _chatRoomParticipantRepository = chatRoomParticipantRepository;
            _expertRepository = expertRepository;
            _chatService = chatService;

        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
                _userConnectionManager.AddUserConnection(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
                _userConnectionManager.RemoveUserConnection(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
        public async Task<long> CreatePrivateRoomAsync(long receiverId)
        {
            var senderId = long.Parse(Context.UserIdentifier!);

            // بررسی روم موجود (چت تکراری نباشد)
            var existingRoom = await _chatRoomRepository.Table
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r =>
                    !r.IsGroup &&
                    r.Participants.Any(p => p.UserId == senderId) &&
                    r.Participants.Any(p => p.UserId == receiverId)
                );

            if (existingRoom != null)
                return existingRoom.Id;

            var room = ChatRoom.CreatePrivateChat(senderId, receiverId);
            await _chatRoomRepository.AddAsync(room, CancellationToken.None);

            return room.Id;
        }
        public async Task<long> CreateGroupRoomAsync(string name, List<long> participantIds, long? userPlanId = null)
        {
            var creatorId = long.Parse(Context.UserIdentifier!);
            if (!participantIds.Contains(creatorId))
                participantIds.Add(creatorId);

            var room = ChatRoom.CreateGroupChat(name, participantIds, userPlanId);
            await _chatRoomRepository.AddAsync(room, CancellationToken.None);

            return room.Id;
        }
        public async Task ReactToMessage(long messageId, ReactionType reactionType)
        {
            var userId = long.Parse(Context.UserIdentifier!);

            var reaction = new ChatMessageReaction(messageId, userId, reactionType);
            var messageInDb = await _chatMessageRepository.Table.Include(t => t.Reactions).Where(t => t.Id == messageId).FirstOrDefaultAsync(CancellationToken.None);
            messageInDb.AddReaction(reaction);
            await _chatMessageRepository.UpdateAsync(messageInDb, CancellationToken.None);
            // ارسال به فرستنده پیام یا همه اعضای چت
            var message = await _chatMessageRepository.Table.Include(m => m.ChatRoom)
                                                            .ThenInclude(r => r.Participants)
                                                            .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null) return;

            foreach (var participant in message.ChatRoom.Participants)
            {
                var conns = _userConnectionManager.GetConnections(participant.UserId.ToString());
                foreach (var conn in conns)
                {
                    await Clients.Client(conn).SendAsync("MessageReacted", new
                    {
                        MessageId = messageId,
                        UserId = userId,
                        ReactionType = reactionType
                    });
                }
            }
        }

        public async Task ForwardMessageAsync(long originalMessageId, long targetRoomId)
        {
            var forwarderId = long.Parse(Context.UserIdentifier!);
            var originalMessage = await _chatMessageRepository.GetByIdAsync(CancellationToken.None, originalMessageId);

            if (originalMessage == null)
                throw new Exception("Message not found");

            var forwarded = originalMessage.ForwardTo(targetRoomId, forwarderId);
            await _chatMessageRepository.AddAsync(forwarded, CancellationToken.None);

            // ارسال پیام به روم مشابه SendMessageAsync
            var messageViewModel = new ChatMessageViewModel
            {
                MessageId = forwarded.Id,
                RoomId = forwarded.ChatRoomId,
                SenderId = forwarded.SenderId,
                EncryptedContent = forwarded.EncryptedContent,
                Type = forwarded.Type,
                ReplyToMessageId = forwarded.ReplyToMessageId,
                CreatedAt = forwarded.CreatedAt
            };

            var room = await _chatRoomRepository.Table.Include(r => r.Participants).FirstOrDefaultAsync(r => r.Id == targetRoomId);

            if (room != null)
            {
                foreach (var member in room.Participants)
                {
                    var connections = _userConnectionManager.GetConnections(member.UserId.ToString());
                    foreach (var connectionId in connections)
                    {

                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", messageViewModel);
                    }

                    await _messageStatusRepository.AddAsync(
                        new MessageStatus(forwarded.Id, member.UserId, ChatMessageStatus.Delivered),
                        CancellationToken.None
                    );
                }
            }
        }
        public async Task<List<ChatMessageViewModel>> LoadMessagesAsync(long roomId, int page = 1, int pageSize = 20)
        {
            var userId = long.Parse(Context.UserIdentifier!);
            var skip = (page - 1) * pageSize;

            // دریافت پیام‌ها به همراه وضعیت‌ها و اطلاعات مرتبط
            var query = _chatMessageRepository.Table
                .Include(t => t.Sender)
                .Include(t => t.Reactions)
                .Where(m => m.ChatRoomId == roomId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(m => new
                {
                    Message = m,
                    UserStatus = _messageStatusRepository.Table.FirstOrDefault(s => s.ReceiverId == userId && s.MessageId == m.Id),
                    SenderName = m.Sender != null ? m.Sender.FullName : null,
                    Reactions = m.Reactions.Select(r => r.ReactionType).ToList()
                });

            var messageData = await query.ToListAsync();

            await UpdateRoomChatMessagesStatusThatAssaginToCurrentUserToRead(userId, roomId);

            // تبدیل به مدل نمایشی
            return messageData.Select(x => new ChatMessageViewModel
            {
                MessageId = x.Message.Id,
                RoomId = x.Message.ChatRoomId,
                SenderId = x.Message.SenderId,
                EncryptedContent = x.Message.EncryptedContent,
                Type = x.Message.Type,
                Status = x.UserStatus?.Status ?? ChatMessageStatus.Sent,
                ReplyToMessageId = x.Message.ReplyToMessageId,
                CreatedAt = x.Message.CreatedAt,
                SenderName = x.SenderName,
                Reactions = x.Reactions
            }).OrderBy(m => m.CreatedAt).ToList();
        }

        private async Task UpdateRoomChatMessagesStatusThatAssaginToCurrentUserToRead(long userId, long roomId)
        {
            // فیلتر پیام‌هایی که باید وضعیتشان به Read به‌روزرسانی شود
            var messagesToUpdateToRead = await _messageStatusRepository.Table
                                            .Where(a => a.ReceiverId == userId &&
                                                        a.Message.ChatRoomId == roomId &&
                                                        a.Status != ChatMessageStatus.Read).ToListAsync();
            // به‌روزرسانی وضعیت‌ها
            if (messagesToUpdateToRead.Any())
            {
                messagesToUpdateToRead.ForEach(x => x.UpdateStatus(ChatMessageStatus.Read));
                await _messageStatusRepository.UpdateRangeAsync(
                    messagesToUpdateToRead.Select(x => x).ToList(), CancellationToken.None);

                // ارسال نوتیفیکیشن به فرستندگان
                var updates = messagesToUpdateToRead.Select(x => new
                {
                    MessageId = x.Message.Id,
                    UserId = userId,
                    Status = ChatMessageStatus.Read
                }).ToList();

                var senderIds = messagesToUpdateToRead
                    .Select(x => x.Message.SenderId)
                    .Distinct()
                    .ToList();

                foreach (var senderId in senderIds)
                {
                    var connections = _userConnectionManager.GetConnections(senderId.ToString());
                    foreach (var connection in connections)
                    {
                        await Clients.Client(connection).SendAsync("MessageStatusUpdated",
                            updates.First(u => messagesToUpdateToRead.Any(m => m.Message.Id == u.MessageId &&
                                                                      m.Message.SenderId == senderId)));
                    }
                }
            }
        }

        /// <summary>
        /// ارسال پیام جدید (متن یا فایل)
        /// </summary>
        public async Task SendMessageAsync(SendMessageDTO messageDto)
        {
            var senderId = long.Parse(Context.UserIdentifier!);

            var message = await _chatService.SendMessageAsync(senderId,
                messageDto.RoomId,
                messageDto.EncryptedContent,
                messageDto.Type,
                messageDto.ReplyToMessageId);

            var messageViewModel = new ChatMessageViewModel
            {
                MessageId = message.Id,
                RoomId = message.ChatRoomId,
                SenderId = message.SenderId,
                EncryptedContent = message.EncryptedContent,
                Type = message.Type,
                ReplyToMessageId = message.ReplyToMessageId,
                CreatedAt = message.CreatedAt
            };

            var room = await _chatRoomRepository.Table.Include(r => r.Participants).FirstOrDefaultAsync(r => r.Id == message.ChatRoomId);

            foreach (var participant in room.Participants)
            {
                var connections = _userConnectionManager.GetConnections(participant.UserId.ToString());

                if (participant.UserId == senderId)
                {
                    messageViewModel.Status = ChatMessageStatus.Sent;
                    foreach (var conn in connections)
                    {
                        await Clients.Client(conn).SendAsync("NewMessage", messageViewModel);

                    }
                }
                else
                {
                    messageViewModel.Status = ChatMessageStatus.Sent; // در لحظه ارسال هنوز فقط "Sent" است
                    foreach (var conn in connections)
                    {
                        await Clients.Client(conn).SendAsync("ReceiveMessage", messageViewModel);
                        await _chatService.UpdateMessageStatusAsync(message.Id, participant.UserId, ChatMessageStatus.Delivered);

                    }
                }
            }
        }


        /// <summary>
        /// اعلام وضعیت خواندن پیام
        /// </summary>
        public async Task UpdateMessageStatusAsync(long messageId, ChatMessageStatus status)
        {
            var userId = long.Parse(Context.UserIdentifier!);
            await _chatService.UpdateMessageStatusAsync(messageId, userId, status);

            var message = await _chatMessageRepository.Table.FirstOrDefaultAsync(m => m.Id == messageId);
            if (message == null || message.SenderId == userId) return;

            var senderConnections = _userConnectionManager.GetConnections(message.SenderId.ToString());
            foreach (var connection in senderConnections)
            {
                await Clients.Client(connection).SendAsync("MessageStatusUpdated", new
                {
                    MessageId = messageId,
                    UserId = userId,
                    Status = status
                });
            }
        }


        public async Task<GlobalGridResult<LoadUserChatsViewModel>> LoadUserChatsAsync(int page = 1, int pageSize = 20, string search = null)
        {

            var userId = long.Parse(Context.UserIdentifier!);
            var userChatRoomIds = _chatRoomParticipantRepository.Table.Where(t => t.UserId == userId).Select(t => t.ChatRoomId);

            var ecpertIds = _expertRepository.Table.Select(t => t.Id);
            var query = _chatRoomRepository.Table
                .Where(t => userChatRoomIds.Contains(t.Id)).SelectMany(t => t.Participants)
                .Where(t =>
                t.UserId != userId &&
                (!string.IsNullOrEmpty(search) ? t.User.FullName.Contains(search) : true)
                );
            var data = await query
                .Select(t => new LoadUserChatsViewModel
                {
                    Chat = new ChatInfoViewModel
                    {
                        ChatId = t.ChatRoomId,
                        ChatRoomName = t.ChatRoom.Name,
                        ChatRoomUserPlanTitle = t.ChatRoom.UserPlan.Plan.Title,
                        IsGroupChatRoom = t.ChatRoom.IsGroup,
                        IsExpert = ecpertIds.Contains(t.UserId)
                    },
                    UserInfo = new ChatUserInfoViewModel
                    {
                        UserId = t.UserId,
                        UserFullName = !string.IsNullOrEmpty(t.User.FullName) ? t.User.FullName : t.User.UserName,
                        IsOnline = _userConnectionManager.GetConnections(t.UserId.ToString()).Any(),
                        ProfileImage = t.User.ProfileImage,
                    },
                    MessageCount = t.ChatRoom.Messages.Count,
                    UnReadMessageCount = _messageStatusRepository.Table
                                            .Count(a => a.ReceiverId == userId &&
                                                        a.Message.ChatRoomId == t.ChatRoomId &&
                                                        a.Status != ChatMessageStatus.Read),
                    LastMessage = t.ChatRoom.Messages.OrderBy(t => t.CreatedAt).Select(t => t.EncryptedContent).LastOrDefault()
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(CancellationToken.None);
            var totalCount = await query.CountAsync(CancellationToken.None);

            return new GlobalGridResult<LoadUserChatsViewModel>
            {
                Data = data,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// پیوستن به یک چت روم (گروهی یا دو نفره)
        /// </summary>
        public async Task JoinRoom(long roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }

        /// <summary>
        /// ترک چت روم
        /// </summary>
        public async Task LeaveRoom(long roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }
    }
}