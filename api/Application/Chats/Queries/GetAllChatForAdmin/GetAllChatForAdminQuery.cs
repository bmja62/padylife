using Application.Chats.Dto;
using Application.Cqrs.Queris;
using Common.GridResults;
using Data.Contracts;
using Entities.Chats;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Chats.Queries.GetAllChatForAdmin
{
    public record GetAllChatForAdminQuery(GetAllChatRequestDto Dto) : IQuery<ServiceResult<GlobalGridResult<LoadChatsForAdminViewModel>>>;


    public class GetAllChatForAdminQueryHandler(IRepository<Expert> _expertRepository, IRepository<ChatRoom> _chatRoomRepository) : IQueryHandler<GetAllChatForAdminQuery, ServiceResult<GlobalGridResult<LoadChatsForAdminViewModel>>>
    {
        public async Task<ServiceResult<GlobalGridResult<LoadChatsForAdminViewModel>>> Handle(GetAllChatForAdminQuery request, CancellationToken cancellationToken)
        {
            var query = _chatRoomRepository.Table
                .Where(cr =>
                    string.IsNullOrEmpty(request.Dto.Search) ||
                    cr.Participants.Any(p => p.User.FullName.Contains(request.Dto.Search))
                );

            var data = await query
                .Select(cr => new LoadChatsForAdminViewModel
                {
                    Chat = new ChatInfoAdminViewModel
                    {
                        ChatId = cr.Id,
                        ChatRoomName = cr.Name,
                        ChatRoomUserPlanTitle = cr.UserPlan != null ? cr.UserPlan.Plan.Title : null,
                        IsGroupChatRoom = cr.IsGroup,
                        UserFullNames = cr.Participants
                            .Select(p => new ChatUsersInfoViewModel
                            {
                                UserId = p.UserId,
                                UserFullName = !string.IsNullOrEmpty(p.User.FullName) ? p.User.FullName : p.User.UserName,
                                ProfileImage = p.User.ProfileImage,
                            })
                            .ToList()
                    },
                    MessageCount = cr.Messages.Count,
                    LastMessage = cr.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.EncryptedContent)
                        .FirstOrDefault(),
                    LastMessageTime = cr.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (DateTime?)m.CreatedAt)
                        .FirstOrDefault()
                })
                .Skip((request.Dto.PageNumber.Value - 1) * request.Dto.Count.Value)
                .Take(request.Dto.Count.Value)
                .ToListAsync(CancellationToken.None);

            var totalCount = await query.CountAsync(CancellationToken.None);

            return ServiceResult.Ok(new GlobalGridResult<LoadChatsForAdminViewModel>
            {
                Data = data,
                TotalCount = totalCount,
            });
        }

    }
}
