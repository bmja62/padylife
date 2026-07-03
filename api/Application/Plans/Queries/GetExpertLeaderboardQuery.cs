using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Common;
using Entities.Rates;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;


namespace Application.Plans.Queries
{
    public record GetExpertLeaderboardQuery(int TopCount = 10)
        : IQuery<ServiceResult<ExpertLeaderboardReportDTO>>;

    public class GetExpertLeaderboardQueryHandler(
         IRepository<Expert> userRepo, IRepository<Rate> RateRepository
     ) : IQueryHandler<GetExpertLeaderboardQuery, ServiceResult<ExpertLeaderboardReportDTO>>
    {
        public async Task<ServiceResult<ExpertLeaderboardReportDTO>> Handle(GetExpertLeaderboardQuery request, CancellationToken cancellationToken)
        {
            // مرحله ۱: گرفتن لیست متخصصانی که Rate دارن
            var expertRates = await RateRepository.TableNoTracking
                .Where(r => r.EntityType == EntityType.Specialist)
                .GroupBy(r => r.EntityId)
                .Select(g => new
                {
                    UserId = g.Key,
                    AvgRate = g.Average(r => r.RatingValue),
                    RateCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            var ratedUserIds = expertRates.Select(x => x.UserId).ToList();

            // مرحله ۲: گرفتن همه متخصص‌ها (برای پر کردن جای خالی‌ها)
            var allExperts = await userRepo.TableNoTracking
                .ToListAsync(cancellationToken);

            // مرحله ۳: ساخت دیکشنری کاربرها
            var users = allExperts.ToDictionary(u => u.Id, u => u);

            // مرحله ۴: لیست کسانی که Rate دارن
            var ratedLeaders = expertRates
                .OrderByDescending(x => x.AvgRate)
                .Select((rate, index) => new EpertLeaderboardUserDTO
                {
                    Rank = index + 1, // موقتی، بعداً بازشماری می‌کنیم
                    UserId = rate.UserId,
                    UserName = users.TryGetValue(rate.UserId, out var user) ? user.FullName : "نامشخص",
                    ProfileImage = user?.ProfileImage,
                    AvgRate = rate.AvgRate
                })
                .ToList();

            // مرحله ۵: اگر تعداد کافی نبود، بقیه رو بیار
            if (ratedLeaders.Count < request.TopCount)
            {
                var needed = request.TopCount - ratedLeaders.Count;

                var nonRatedExperts = allExperts
                    .Where(u => !ratedUserIds.Contains(u.Id))
                    .OrderBy(u => u.FullName) // مرتب‌سازی الفبا
                    .Take(needed)
                    .Select(u => new EpertLeaderboardUserDTO
                    {
                        UserId = u.Id,
                        UserName = u.FullName,
                        ProfileImage = u.ProfileImage,
                        AvgRate = 0
                    })
                    .ToList();

                ratedLeaders.AddRange(nonRatedExperts);
            }

            // مرحله ۶: نهایی‌سازی رنک (۱ تا N)
            int rank = 1;
            foreach (var leader in ratedLeaders)
            {
                leader.Rank = rank++;
            }

            // مرحله ۷: بریدن فقط TopCount
            var leaders = ratedLeaders.Take(request.TopCount).ToList();

            // خروجی
            var result = new ExpertLeaderboardReportDTO
            {
                Leaders = leaders,
                GeneratedAt = DateTime.UtcNow
            };

            return ServiceResult.Ok(result);
        }





        /*         var expertRates = await RateRepository.TableNoTracking
           .Where(r => r.EntityType == EntityType.Specialist)
           .GroupBy(r => r.EntityId) 
           .Select(g => new
           {
               UserId = g.Key,
               AvgRate = g.Average(r => r.RatingValue),
               RateCount = g.Count()
           })
           .ToListAsync(cancellationToken);

                 if (!expertRates.Any())
                     return ServiceResult.Ok(new ExpertLeaderboardReportDTO
                     {
                         Leaders = new List<EpertLeaderboardUserDTO>(),
                         GeneratedAt = DateTime.UtcNow
                     });


                 var userIds = expertRates.Select(x => x.UserId).ToList();


                 var users = await userRepo.TableNoTracking
                     .Where(u => userIds.Contains(u.Id))
                     .ToDictionaryAsync(u => u.Id, cancellationToken);


                 var leaders = new List<EpertLeaderboardUserDTO>();
                 int rank = 1;

                 foreach (var rate in expertRates.OrderByDescending(x => x.AvgRate))
                 {
                     users.TryGetValue(rate.UserId, out var user);

                     leaders.Add(new EpertLeaderboardUserDTO
                     {
                         Rank = rank++,
                         UserId = rate.UserId,
                         UserName = user?.FullName ?? "نامشخص",
                         ProfileImage = user?.ProfileImage,
                         AvgRate = rate.AvgRate
                     });
                 }

                 // مرحله ۴: ساخت خروجی
                 var result = new ExpertLeaderboardReportDTO
                 {
                     Leaders = leaders,
                     GeneratedAt = DateTime.UtcNow
                 };

                 return ServiceResult.Ok(result);
     */

    }
}
