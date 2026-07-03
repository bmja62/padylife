using Application.Cqrs.Queris;
using Application.Payments.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Payments.Queries.GetPayments
{
    public class GetPaymentsQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<PaymentDTO>>>
    {
        public GetPaymentsQuery(int pageNumber, int count, bool? isPayed, DateTime? from, DateTime? to, string userFullName, long? userId)
        {
            PageNumber = pageNumber;
            Count = count;
            IsPayed = isPayed;
            From = from;
            To = to;
            UserFullName = userFullName;
            UserId = userId;
        }

        public bool? IsPayed { get; }
        public DateTime? From { get; }
        public DateTime? To { get; }
        public string UserFullName { get; }
        public long? UserId { get; }
    }

    public class GetPaymentsQueryHandler
        (IRepository<Payment> repository)
        : IQueryHandler<GetPaymentsQuery, ServiceResult<GlobalGridResult<PaymentDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<PaymentDTO>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var query = repository
                .Table.Include(t => t.User)
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if (request.IsPayed.HasValue)
                query = query.Where(x => x.IsPaid == request.IsPayed);

            if (!string.IsNullOrEmpty(request.UserFullName))
                query = query.Where(x => (x.User.FullName).Contains(request.UserFullName));

            if (request.From.HasValue)
                query = query.Where(x => x.CreatedOn >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(x => x.CreatedOn <= request.To.Value);

            if (request.UserId.HasValue)
                query = query.Where(x => x.UserId == request.UserId);


            var payments =
                await query.OrderByDescending(t => t.CreatedOn)
                .Select(x => new PaymentDTO
                {
                    Amount = x.Amount,
                    IsPayed = x.IsPaid,
                    OrderId = x.OrderId,
                    PayedAt = x.UpdatedOn,
                    CreatedAt = x.CreatedOn,
                    UserId = x.UserId.GetValueOrDefault(),
                    WalletCharge = x.WalletCharge,
                    User = new PaymentUserDTO
                    {
                        Id = x.Id,
                        FullName = x.User.FullName ?? x.User.UserName,
                        PhoneNumber = x.User.PhoneNumber
                    }
                })
                .Skip((request.PageNumber.Value - 1) * request.Count.Value)
                .Take(request.Count.Value)
                .ToListAsync(cancellationToken);

            var totalCounts = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<PaymentDTO>
            {
                Data = payments,
                TotalCount = totalCounts
            });
        }
    }
}
