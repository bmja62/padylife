using Common;
using Common.GridResults;
using Data.Contracts;
using Entities.Comments;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Services.Services.CommentServices.DTOs;

namespace Services.Services.CommentServices
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(long userId, CreateCommentDTO dto);
        Task ReactToCommentAsync(long commentId, long userId, ReactionType type);
        Task<GlobalGridResult<GetCommentDTO>> GetEntityCommentsAsync(long? entityId, bool? isApproved, EntityType? entityType, int skip, int take, long userId);
        Task<bool> DeleteCommentAsync(long commentId, long userId);
        Task<Comment> EditCommentAsync(long commentId, long userId, string newText);
        Task<bool> ApproveCommentAsync(long commentId);
        Task<int> GetCountAsync(EntityType? entityType);
    }
    public class CommentService : ICommentService, IScopedDependency
    {
        private readonly IRepository<Comment> _commentRepository;

        public CommentService(IRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Comment> AddCommentAsync(long userId, CreateCommentDTO dto)
        {
            var comment = new Comment(dto.EntityId, dto.EntityType, userId, dto.Text, dto.ParentCommentId);
            await _commentRepository.AddAsync(comment, CancellationToken.None);

            return comment;
        }

        public async Task ReactToCommentAsync(long commentId, long userId, ReactionType type)
        {
            var comment = await _commentRepository.Table
                .Include(c => c.Reactions)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) throw new Exception("Comment not found");

            comment.React(userId, type);
            await _commentRepository.UpdateAsync(comment, CancellationToken.None);
        }

        public async Task<GlobalGridResult<GetCommentDTO>> GetEntityCommentsAsync(long? entityId, bool? isApproved, EntityType? entityType, int skip, int take, long userId)

        {
            var Query = _commentRepository.Table
                .Where(c => entityId.HasValue ? c.EntityId == entityId : true)
                .Where(c => entityType.HasValue ? c.EntityType == entityType && c.ParentCommentId == null : true)
                .Where(c => isApproved.HasValue ? c.IsApproved == isApproved : true)
                .Include(t => t.Reactions)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Reactions)
                .Include(c => c.Reactions)
                .Select(t => new GetCommentDTO
                {
                    Id = t.Id,
                    Text = t.Text,
                    LikeCount = t.LikeCount,
                    DislikeCount = t.DislikeCount,
                    IsApproved = t.IsApproved,
                    IsMe = t.CreatedByUserId == userId,
                    IsLikedByMe = t.Reactions.Any(a => a.UserId == userId && a.ReactionType == ReactionType.Like),
                    IsReactedByLoginUser = t.Reactions.Any(a => a.UserId == userId),
                    UserInfo = new GetCommentUserInfoDTO
                    {
                        Id = t.CreatedByUser.Id,
                        FullName = t.CreatedByUser.FullName
                    },
                    Replies = t.Replies.Select(tt => new GetCommentDTO
                    {
                        Id = tt.Id,
                        Text = tt.Text,
                        LikeCount = tt.LikeCount,
                        DislikeCount = tt.DislikeCount,
                        IsApproved = tt.IsApproved,
                        IsMe = t.CreatedByUserId == userId,
                        IsLikedByMe = t.CreatedByUserId == userId && t.Reactions.Any(a => a.UserId == userId && a.ReactionType == ReactionType.Like),
                        IsReactedByLoginUser = t.Reactions.Any(a => a.UserId == userId),
                        UserInfo = new GetCommentUserInfoDTO
                        {
                            Id = tt.CreatedByUser.Id,
                            FullName = tt.CreatedByUser.FullName
                        },
                    })
                })
                   ;
            var Count = await Query.CountAsync();

            var Data = await Query.Skip(skip)
             .Take(take)
             .ToListAsync();
            var Result = new GlobalGridResult<GetCommentDTO>()
            {
                Data = Data,
                TotalCount = Count,
            };
            return Result;
        }

        public async Task<int> GetCountAsync(EntityType? entityType) => await _commentRepository.Table.Where(t => entityType.HasValue ? t.EntityType == entityType : true).CountAsync();

        public async Task<bool> DeleteCommentAsync(long commentId, long userId)
        {
            var comment = await _commentRepository.Table
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null /*|| comment.CreatedByUserId != userId*/)
                return false;

            await _commentRepository.SoftDeleteAsync(comment, CancellationToken.None);
            return true;
        }

        public async Task<bool> ApproveCommentAsync(long commentId)
        {
            var comment = await _commentRepository.Table.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
                throw new Exception("Comment not found.");

            comment.ApproveComment();
            await _commentRepository.UpdateAsync(comment, CancellationToken.None);

            return true;
        }

        public async Task<Comment> EditCommentAsync(long commentId, long userId, string newText)
        {
            var comment = await _commentRepository.Table.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null || comment.CreatedByUserId != userId)
                throw new Exception("Comment not found or user is not the owner.");

            comment.EditComment(newText);
            await _commentRepository.UpdateAsync(comment, CancellationToken.None);
            return comment;
        }

    }

}
