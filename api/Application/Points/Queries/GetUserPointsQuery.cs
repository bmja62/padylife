using Application.Cqrs.Queris;
using Application.Points.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Points.Queries
{
    public class GetUserPointsQuery(long userId) : IQuery<ServiceResult<UserPointsDTO>>
    {
        public long UserId { get; } = userId;
    }

    public class GetUserPointsQueryHandler(IRepository<UserPoints> userPointsRepository)
        : IQueryHandler<GetUserPointsQuery, ServiceResult<UserPointsDTO>>
    {
        public async Task<ServiceResult<UserPointsDTO>> Handle(GetUserPointsQuery request, CancellationToken cancellationToken)
        {
            var userPoints = await userPointsRepository.Table
                .Include(up => up.PointTransactions)
                .FirstOrDefaultAsync(up => up.UserId == request.UserId, cancellationToken);

            if (userPoints == null)
            {
                var newUserPoint = new UserPoints(request.UserId);
                await userPointsRepository.AddAsync(newUserPoint, cancellationToken);
                userPoints = newUserPoint;
            }


            var dto = new UserPointsDTO
            {
                UserId = userPoints.UserId,
                AvailablePoints = userPoints.AvailablePoints,
                EarnedPoints = userPoints.EarnedPoints,
                ConsumedPoints = userPoints.ConsumedPoints,
                PointsToMoneyRatio = UserPoints.PointsToMoneyRatio,
                MoneyValue = userPoints.CalculateMoneyValue(userPoints.AvailablePoints)
            };

            return ServiceResult.Ok(dto);
        }
    }
}
