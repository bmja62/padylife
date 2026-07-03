using Application.Cqrs.Queris;
using Common.GridResults;
using Common.Utilities;
using Entities.Common;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;
using Services.Services.CommentServices.DTOs;

namespace Application.Comments.Queries
{
    public record GetEntityCommentsQuery(long? EntityId, bool? IsApproved, EntityType? EntityType, GlobalGrid globalGrid)
     : IQuery<ServiceResult<GlobalGridResult<GetCommentDTO>>>;

    public class GetEntityCommentsQueryHandler(
        IHttpContextAccessor accessor,
        ICommentService commentService
        )
        : IQueryHandler<GetEntityCommentsQuery, ServiceResult<GlobalGridResult<GetCommentDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetCommentDTO>>> Handle(GetEntityCommentsQuery request, CancellationToken cancellationToken) => ServiceResult.Ok(
              await commentService.GetEntityCommentsAsync(
                request.EntityId,
                request.IsApproved,
                request.EntityType,
                request.globalGrid.Skip,
                request.globalGrid.Take,
                accessor.HttpContext.User.Identity.GetUserId<long>()
                )

        );
    }
}
