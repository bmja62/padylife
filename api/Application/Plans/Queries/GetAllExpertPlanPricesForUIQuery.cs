using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Common;
using Entities.Plans;
using Entities.Rates;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetAllExpertPlanPricesForUIQuery(GlobalGrid GlobalGrid, bool? ExpertCompanions, long? planId, bool? RateFilter) : IQuery<ServiceResult<GlobalGridResult<ExpertPlanPriceForUIDTO>>>;
    public class GetAllExpertPlanPricesForUIQueryHandler : IQueryHandler<GetAllExpertPlanPricesForUIQuery, ServiceResult<GlobalGridResult<ExpertPlanPriceForUIDTO>>>
    {
        private readonly IRepository<ExpertPlanPrice> _repository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository<Rate> _rateRepository;
        private readonly IRepository<Expert> _expertRepository;
        private readonly IRepository<Plan> _planRepository;
        private readonly IRepository<UserPlanExpert> _userPlanExpertRepository;

        public GetAllExpertPlanPricesForUIQueryHandler(IRepository<ExpertPlanPrice> repository, IHttpContextAccessor accessor, IRepository<Plan> planRepository, IRepository<Expert> expertRepository, IRepository<Rate> rateRepository, IRepository<UserPlanExpert> userPlanExpertRepository)
        {
            _userPlanExpertRepository = userPlanExpertRepository;
            _rateRepository = rateRepository;
            _planRepository = planRepository;
            _repository = repository;
            _expertRepository = expertRepository;
            _accessor = accessor;
        }

        public async Task<ServiceResult<GlobalGridResult<ExpertPlanPriceForUIDTO>>> Handle(GetAllExpertPlanPricesForUIQuery request, CancellationToken cancellationToken)
        {

            var userId = _accessor.HttpContext.User.Identity.GetUserId<long>();
            var baseQuery = _repository.Table
    .Where(t => t.IsActive);

            if (request.planId.HasValue)
                baseQuery = baseQuery.Where(t => t.PlanId == request.planId.Value);

            IQueryable<ExpertPlanPrice> orderedQuery;

            if (request.RateFilter.HasValue)
            {
                // برای مرتب‌سازی جداگانه باید از Join/Subquery استفاده کنیم
                var avgRatesQuery = _rateRepository.TableNoTracking
                    .Where(r => r.EntityType == EntityType.Specialist)
                    .GroupBy(r => r.EntityId)
                    .Select(g => new
                    {
                        ExpertId = g.Key,
                        AvgRate = g.Average(r => (double?)r.RatingValue) ?? 0
                    });

                orderedQuery = request.RateFilter.Value
                    ? baseQuery
                        .GroupJoin(avgRatesQuery, p => p.ExpertId, ar => ar.ExpertId, (p, ar) => new { p, ar })
                        .SelectMany(x => x.ar.DefaultIfEmpty(), (x, ar) => new { x.p, AvgRate = ar.AvgRate })
                        .OrderByDescending(x => x.AvgRate)
                        .Select(x => x.p)
                    : baseQuery
                        .GroupJoin(avgRatesQuery, p => p.ExpertId, ar => ar.ExpertId, (p, ar) => new { p, ar })
                        .SelectMany(x => x.ar.DefaultIfEmpty(), (x, ar) => new { x.p, AvgRate = ar.AvgRate })
                        .OrderBy(x => x.AvgRate)
                        .Select(x => x.p);
            }
            else if (request.ExpertCompanions.HasValue)
            {
                var companionsQuery = _userPlanExpertRepository.TableNoTracking
                    .Where(up => up.UserPlan.IsCompleted)
                    .GroupBy(up => up.ExpertId)
                    .Select(g => new
                    {
                        ExpertId = g.Key,
                        CompanionsCount = g.Count()
                    });

                orderedQuery = request.ExpertCompanions.Value
                    ? baseQuery
                        .GroupJoin(companionsQuery, p => p.ExpertId, c => c.ExpertId, (p, c) => new { p, c })
                        .SelectMany(x => x.c.DefaultIfEmpty(), (x, c) => new { x.p, CompanionsCount = c.CompanionsCount })
                        .OrderByDescending(x => x.CompanionsCount)
                        .Select(x => x.p)
                    : baseQuery
                        .GroupJoin(companionsQuery, p => p.ExpertId, c => c.ExpertId, (p, c) => new { p, c })
                        .SelectMany(x => x.c.DefaultIfEmpty(), (x, c) => new { x.p, CompanionsCount = c.CompanionsCount })
                        .OrderBy(x => x.CompanionsCount)
                        .Select(x => x.p);
            }
            else
            {
                orderedQuery = baseQuery.OrderBy(a => a.ExpertId);
            }

            // تعداد کل
            var totalCountTask = await orderedQuery.CountAsync(cancellationToken);

            // داده صفحه‌بندی شده
            var data = await orderedQuery
                .Skip(request.GlobalGrid.Skip)
                .Take(request.GlobalGrid.Take)
                .Select(p => new ExpertPlanPriceForUIDTO
                {
                    Id = p.Id,
                    ExpertId = p.ExpertId,
                    ExpertFullName = "-",
                    PlanId = p.PlanId,
                    PlanTitle = "-",
                    Price = p.Price,

                    IsActive = p.IsActive,
                    JobTitle = "-",
                    AverageRate = 0,
                    CompanionsCount = 0
                })
                .ToListAsync(cancellationToken);

            // گرفتن دیتای جانبی به صورت Batch
            var expertIds = data.Select(d => d.ExpertId).Distinct().ToList();
            var planIds = data.Select(d => d.PlanId).Distinct().ToList();

            // لود نام کارشناس
            var experts = await _expertRepository.TableNoTracking
                .Where(e => expertIds.Contains(e.Id))
                .Select(e => new { e.Id, e.FullName, e.JobTitle })
                .ToListAsync(cancellationToken);

            // لود نام پلن
            var plans = await _planRepository.TableNoTracking
                .Where(p => planIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Title })
                .ToListAsync(cancellationToken);

            // لود AverageRate
            var rates = await _rateRepository.TableNoTracking
                .Where(r => expertIds.Contains(r.EntityId) && r.EntityType == EntityType.Specialist)
                .GroupBy(r => r.EntityId)
                .Select(g => new { ExpertId = g.Key, AvgRate = g.Average(r => (double?)r.RatingValue) ?? 0 })
                .ToListAsync(cancellationToken);

            // لود CompanionsCount
            var companions = await _userPlanExpertRepository.TableNoTracking
                .Where(up => expertIds.Contains(up.ExpertId) && up.UserPlan.IsCompleted)
                .GroupBy(up => up.ExpertId)
                .Select(g => new { ExpertId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // پر کردن دیتا
            foreach (var d in data)
            {
                var expert = experts.FirstOrDefault(e => e.Id == d.ExpertId);
                var plan = plans.FirstOrDefault(p => p.Id == d.PlanId);
                var rate = rates.FirstOrDefault(r => r.ExpertId == d.ExpertId);
                var comp = companions.FirstOrDefault(c => c.ExpertId == d.ExpertId);

                d.ExpertFullName = expert?.FullName ?? "-";
                d.JobTitle = expert?.JobTitle ?? "-";
                d.PlanTitle = plan?.Title ?? "-";
                d.AverageRate = rate?.AvgRate ?? 0;
                d.CompanionsCount = comp?.Count ?? 0;
            }

            return ServiceResult.Ok(new GlobalGridResult<ExpertPlanPriceForUIDTO>
            {
                Data = data,
                TotalCount = totalCountTask
            });

            /*        var baseQuery = _repository.Table
                        .Include(t => t.Expert)
                            .ThenInclude(e => e.Rates)
                        .Include(t => t.Expert)
                            .ThenInclude(e => e.UserPlanExperts)
                                .ThenInclude(up => up.UserPlan)
                        .Include(t => t.Plan)
                        .Where(t => t.IsActive);

                    if (request.planId.HasValue)
                        baseQuery = baseQuery.Where(t => t.PlanId == request.planId.Value);

                    IOrderedQueryable<ExpertPlanPrice> orderedQuery;

                    if (request.RateFilter.HasValue)
                    {
                        orderedQuery = request.RateFilter.Value
                            ? baseQuery.OrderByDescending(a => a.Expert.Rates.Any()
                                ? a.Expert.Rates.Average(r => r.RatingValue)
                                : 0)
                            : baseQuery.OrderBy(a => a.Expert.Rates.Any()
                                ? a.Expert.Rates.Average(r => r.RatingValue)
                                : 0);
                    }
                    else if (request.ExpertCompanions.HasValue)
                    {
                        orderedQuery = request.ExpertCompanions.Value
                            ? baseQuery.OrderByDescending(a => a.Expert.UserPlanExperts.Count(up => up.UserPlan.IsCompleted))
                            : baseQuery.OrderBy(a => a.Expert.UserPlanExperts.Count(up => up.UserPlan.IsCompleted));
                    }
                    else
                    {
                        orderedQuery = baseQuery.OrderBy(a => a.ExpertId);
                    }

                    var totalCountTask = orderedQuery.CountAsync(cancellationToken);


                    var dataTaask = orderedQuery
            .Skip(request.GlobalGrid.Skip)
            .Take(request.GlobalGrid.Take)
            .Select(p => new ExpertPlanPriceForUIDTO
            {
                Id = p.Id,
                ExpertId = p.ExpertId,
                ExpertFullName = p.Expert != null ? (p.Expert.FullName ?? "-") : "-",
                PlanId = p.PlanId,
                PlanTitle = p.Plan != null ? p.Plan.Title : "-",
                Price = p.Price,
                IsActive = p.IsActive,
                JobTitle = p.Expert != null ? p.Expert.JobTitle ?? "-" : "-",
                AverageRate = _rateRepository.TableNoTracking
           .Where(r => r.EntityId == p.ExpertId && r.EntityType == EntityType.Specialist)
           .Average(r => (double?)r.RatingValue) ?? 0.0,

                CompanionsCount = _userPlanExpertRepository.TableNoTracking
           .Count(up => up.ExpertId == p.ExpertId && up.UserPlan.IsCompleted)
            })
            .ToListAsync();

                    return ServiceResult.Ok(new GlobalGridResult<ExpertPlanPriceForUIDTO>
                    {
                        Data = await dataTaask,
                        TotalCount = await totalCountTask
                    });
                }
        */
        }
    }
}


