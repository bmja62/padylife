using Application.Cqrs.Queris;
using Application.Plans.Helpers;
using Application.Questions.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Services;

namespace Application.Plans.Queries
{
    public class GetUserPlansStatusQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<UserPlanStatusDTO>>>
    {
        public GetUserPlansStatusQuery(long userId, GlobalGrid globalGrid, long? planId)
        {
            UserId = userId;
            PlanId = planId;
            PageNumber = globalGrid.PageNumber;
            Count = globalGrid.Count;

        }
        public long UserId { get; }
        public long? PlanId { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userRepository"></param>
    public class GetUserPlansQueryHandler(
        IRepository<UserPlan> userPlansQuery,
        IRepository<UserPlanAnswer> userPlanAnswersQuery,
        IRepository<QuestionLinked> questionLinksQuery,
        IRepository<PlanQuestion> planQuestionsQuery,
        IRepository<Plan> planQuery
        ) : IQueryHandler<GetUserPlansStatusQuery, ServiceResult<GlobalGridResult<UserPlanStatusDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<UserPlanStatusDTO>>> Handle(GetUserPlansStatusQuery request, CancellationToken cancellationToken)
        => ServiceResult.Ok(new GlobalGridResult<UserPlanStatusDTO>
        {
            Data = (await PlanFlowService.GetUserPlansStatus
            (
                userPlansQuery.Table,
                userPlanAnswersQuery.Table,
                questionLinksQuery.Table,
                planQuestionsQuery.Table,
                planQuery.Table,
                request.UserId,
                request.PlanId
            )).Skip((request.PageNumber.Value - 1) * request.Count.Value)
            .Take(request.Take)
            .ToList(),
            TotalCount = (await PlanFlowService.GetUserPlansStatus
            (
                userPlansQuery.Table,
                userPlanAnswersQuery.Table,
                questionLinksQuery.Table,
                planQuestionsQuery.Table,
                planQuery.Table,
                request.UserId,
                request.PlanId
            )).Count

        });
    }
}
