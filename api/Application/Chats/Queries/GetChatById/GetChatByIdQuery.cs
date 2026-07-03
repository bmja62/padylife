using Application.Chats.Dto;
using Application.Cqrs.Queris;
using Common.GridResults;
using Data.Contracts;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Hubs.DTOs;

namespace Application.Chats.Queries.GetChatById
{
    public record GetChatByIdQuery(ChatByIdRequestDto Dto) : IQuery<ServiceResult<GlobalGridResult<ChatMessageViewModel>>>;
    public class GetChatByIdQueryHandler(IRepository<ChatMessage> _chatMessageRepository) : IQueryHandler<GetChatByIdQuery, ServiceResult<GlobalGridResult<ChatMessageViewModel>>>
    {
        public async Task<ServiceResult<GlobalGridResult<ChatMessageViewModel>>> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            var skip = (request.Dto.PageNumber.Value - 1) * request.Dto.Count.Value;

            var query = _chatMessageRepository.Table
             .Include(t => t.Sender)
             .Include(t => t.Reactions)
             .Where(m => m.ChatRoomId == request.Dto.RoomId)
             .OrderByDescending(m => m.CreatedAt);
            var totalCount = await query.CountAsync(cancellationToken);
            var result = await query.Skip(skip)
             .Take(request.Dto.Count.Value)
             .Select(m => new ChatMessageViewModel
             {
                 MessageId = m.Id,
                 RoomId = m.ChatRoomId,
                 SenderId = m.SenderId,
                 EncryptedContent = m.EncryptedContent,
                 Type = m.Type,
                 ReplyToMessageId = m.ReplyToMessageId,
                 CreatedAt = m.CreatedAt,
                 SenderName = m.Sender != null ? m.Sender.FullName : null,
                 Reactions = m.Reactions.Select(r => r.ReactionType).ToList()
             }).ToListAsync(cancellationToken);
            return ServiceResult.Ok(new GlobalGridResult<ChatMessageViewModel>
            {
                Data = result,
                TotalCount = totalCount,
            });


        }
    }
}


