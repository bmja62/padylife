using Application.Cqrs.Queris;
using Common.Utilities;
using Data.Contracts;
using Entities.Excersies;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public class GetUserPlanExercisesQuery(long? userPlanId) : IQuery<ServiceResult<object>>
    {
        public long? UserPlanId { get; } = userPlanId;

    }

    public class GetUserPlanExercisesQueryHandler
        (
        IHttpContextAccessor accessor,
        IRepository<Exercise> exerciseRepository,
        IRepository<UserExercise> userExerciseRepository,
        IRepository<UserPlanExpert> userPlanExpertRepository
        ) : IQueryHandler<GetUserPlanExercisesQuery, ServiceResult<object>>
    {
        public async Task<ServiceResult<object>> Handle(GetUserPlanExercisesQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = userExerciseRepository.Table
                .Include(t => t.UserPlan)
                    .ThenInclude(t => t.Plan)
                .Include(t => t.UserPlan)
                    .ThenInclude(t => t.Experts)
                .Include(t => t.Exercise)
                    .AsQueryable();

            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();

            if (currentUserId > 0)
                baseQuery = baseQuery.Where(t => t.UserId == currentUserId);

            if (request.UserPlanId.HasValue && request.UserPlanId.Value > 0)
                baseQuery = baseQuery.Where(t => t.UserPlanId == request.UserPlanId.Value);

            var groupByQuery = baseQuery.GroupBy(t => new
            {
                UserPlanId = t.UserPlanId,
                PlanId = t.UserPlan.Plan.Id,
                PlanTitle = t.UserPlan.Plan.Title,
                UserId = t.UserId,
                UserFullName = t.User.FullName,
                IsCompleted = t.IsCompleted,
                PlanLevel = t.UserPlan.Plan.Level
            });
            var data = await groupByQuery.Select(tt => new
            {
                tt.Key.UserPlanId,
                tt.Key.PlanId,
                tt.Key.IsCompleted,
                tt.Key.PlanTitle,
                tt.Key.UserId,
                tt.Key.UserFullName,
                tt.Key.PlanLevel,
                Exercises = tt.Select(ttt => new
                {
                    ttt.ExerciseId,
                    ttt.Exercise.Id,
                    ttt.Exercise.Title,
                    ttt.Exercise.DocumentLink,
                    ttt.Exercise.ExerciseEstimate,
                    ttt.Exercise.ExerciseGoal,
                    ttt.Exercise.ExerciseType,
                    ttt.Exercise.PracticeMethod,
                    ttt.Exercise.ExerciseSteps.Count,
                }).ToList(),
                PlanExperts = userPlanExpertRepository.Table.Where(upe => upe.UserPlanId == tt.Key.UserPlanId).Select(tt => new
                {
                    ExpertId = tt.ExpertId,
                    FullName = tt.Expert.FullName,
                    Specialization = tt.Specialization,
                    ProfileImage = tt.Expert.ProfileImage,
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);

            return ServiceResult.Ok<object>(data);
        }
    }
}
