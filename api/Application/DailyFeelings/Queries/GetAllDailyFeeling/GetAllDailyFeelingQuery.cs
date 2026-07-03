using Application.Cqrs.Queris;
using Application.DailyFeelings.DTOs;
using Common.GridResults;
using Data.Contracts;
using Data.Repositories.Extentions;
using Entities.DailyFeelings;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.DailyFeelings.Queries.GetAllDailyFeeling
{
    public class GetAllDailyFeelingQuery : IQuery<ServiceResult<GlobalGridResult<GetAllDailyFeelingDTO>>>
    {
        public GetAllDailyFeelingQuery(GetAllDailyFeelingQueryDTO input)
        {
            Input = input;
        }

        public GetAllDailyFeelingQueryDTO Input { get; }
    }
    public class GetAllDailyFeelingQueryHandler(IRepository<DailyFeeling> dailyFeelingRepository) : IQueryHandler<GetAllDailyFeelingQuery, ServiceResult<GlobalGridResult<GetAllDailyFeelingDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetAllDailyFeelingDTO>>> Handle(GetAllDailyFeelingQuery request, CancellationToken cancellationToken)
        {
            var query = dailyFeelingRepository.TableNoTracking.OrderByDescending(a => a.CreatedAt).AsQueryable();
            if (request.Input.Id.HasValue && request.Input.Id.Value > 0)
                query = query.Where(t => t.Id == request.Input.Id.Value);
            if (request.Input.Type.HasValue)
                query = query.Where(t => t.Type == request.Input.Type.Value);
            if (request.Input.StartDate.HasValue)
                query = query.Where(t => t.CreatedAt >= request.Input.StartDate);
            if (request.Input.EndDate.HasValue)
                query = query.Where(t => t.CreatedAt <= request.Input.EndDate);
            if (request.Input.UserId.HasValue)
                query = query.Where(t => t.CreatedByUserId == request.Input.UserId);

            var data = await query.ApplyPagination(request.Input.PageNumber.Value, request.Input.Count.Value).Select(t => new GetAllDailyFeelingDTO
            {
                Id = t.Id,
                Type = t.Type,
                Description = t.Description,
                VoiceUrl = t.VoiceUrl,
                CreatedAt = t.CreatedAt,
                UserInfo = new GetAllDailyFeelingUserInfoDTO
                {
                    CreatedByUserId = t.CreatedByUserId,
                    UserName = t.CreatedByUser.UserName,
                    FullName = t.CreatedByUser.FullName,
                    Age = t.CreatedByUser.Age,
                    Email = t.CreatedByUser.Email,
                },

            }).ToListAsync();

            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetAllDailyFeelingDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
