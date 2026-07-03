using Common;
using Common.GridResults;
using Data.Contracts;
using Entities.Medals;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.MedalServices.DTOs;

namespace Services.Services.MedalServices
{
    public interface IMedalService
    {
        Task CheckAndAssignMedalsAsync(long userId);
        Task<ServiceResult<GlobalGridResult<GetMetalDTO>>> GetAll(int pageNumber, int count, long userId);
    }

    public class MedalService : IScopedDependency, IMedalService
    {
        private readonly IRepository<Medal> _medalRepo;
        private readonly IRepository<UserMedal> _userMedalRepo;
        private readonly IRepository<User> _userRepo;
        private readonly MedalConditionFactory _conditionFactory;

        public MedalService(
            IRepository<Medal> medalRepo,
            IRepository<UserMedal> userMedalRepo,
            IRepository<User> userRepo,
            MedalConditionFactory conditionFactory)
        {
            _medalRepo = medalRepo;
            _userMedalRepo = userMedalRepo;
            _userRepo = userRepo;
            _conditionFactory = conditionFactory;
        }

        public async Task CheckAndAssignMedalsAsync(long userId)
        {
            var userQuery = _userRepo.Table
                .Include(u => u.UserPlans)
                .Include(u => u.Orders)
                .Where(u => u.Id == userId);

            var user = await userQuery.FirstOrDefaultAsync();
            if (user == null)
                return;

            var medals = await _medalRepo.Table
                .Include(m => m.Conditions)
                .ToListAsync();

            foreach (var medal in medals)
            {
                var compositeCondition = _conditionFactory.CreateCompositeCondition(medal.Conditions);
                var predicate = compositeCondition.ToExpression();

                // بررسی شرایط روی user در حافظه
                var isSatisfied = predicate.Compile()(user);

                if (isSatisfied)
                {
                    var alreadyHas = await _userMedalRepo.Table
                        .AnyAsync(m => m.UserId == userId && m.MedalId == medal.Id);

                    if (!alreadyHas)
                    {
                        var userMedal = new UserMedal
                        {
                            UserId = userId,
                            MedalId = medal.Id,
                            AwardedAt = DateTime.UtcNow
                        };

                        await _userMedalRepo.AddAsync(userMedal, CancellationToken.None);
                    }
                }
            }
        }

        public async Task<ServiceResult<GlobalGridResult<GetMetalDTO>>> GetAll(int pageNumber, int count, long userId) => ServiceResult.Ok(new GlobalGridResult<GetMetalDTO>
        {
            Data = await _medalRepo.Table
                .Include(a => a.UserMedals)
                .Skip(((pageNumber - 1) * count)).Take(count).Select(t => new GetMetalDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IconUrl = t.IconUrl,
                    IsLocked = !t.UserMedals.Any(a => a.UserId == userId && a.MedalId == t.Id)
                }).ToListAsync(),
            TotalCount = await _medalRepo.Table.CountAsync(),
        });
    }


}
