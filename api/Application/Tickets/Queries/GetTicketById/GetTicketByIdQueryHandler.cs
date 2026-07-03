using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;


namespace Application.Tickets.Queries.GetTicketById
{
    public class GetTicketByIdQueryHandler(IRepository<Ticket> repository, IHttpContextAccessor httpContextAccessor)
        : IQueryHandler<GetTicketByIdQuery, ServiceResult<TicketDTO>>
    {
        public async Task<ServiceResult<TicketDTO>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket =
                await
                repository
                .Table
                .Where(x => x.Id == request.TicketId)
                .Select(x => new TicketDTO
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Status = x.Status,
                    TicketType = x.TicketType,
                    Title = x.Title,
                    UserId = x.UserId,
                    TicketDetails = x.TicketDetails
                    .Select(d => new TicketDetailDTO
                    {
                        Id = d.Id,
                        Content = d.Content,
                        CreatedAt = d.CreatedAt,
                        ResponseType = d.ResponseType,
                        SupportUserId = d.SupportUserId,
                    }).ToList(),
                }).FirstOrDefaultAsync(cancellationToken);


            var isAdmin = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.Name);
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            if (ticket is null)
                ServiceResult.BadRequest("تیکت یافت نشد");

            if (ticket.UserId != userId && !isAdmin)
                return ServiceResult.BadRequest<TicketDTO>("شما دسترسی به این منبع ندارید");

            return ServiceResult.Ok(ticket);

        }
    }
}
