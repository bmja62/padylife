using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Helpers;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Services;

namespace Application.Plans.Queries
{
    public class GetNextUnansweredQuestionQuery : IQuery<ServiceResult<GetByIdQuestionDTO>>
    {
        public GetNextUnansweredQuestionQuery(GetNextUnansweredQuestionDTO getNextUnansweredQuestionDTO)
        {
            GetNextUnansweredQuestionDTO = getNextUnansweredQuestionDTO;
        }

        public GetNextUnansweredQuestionDTO GetNextUnansweredQuestionDTO { get; }
    }

    public class GetNextUnansweredQuestionQueryHandler(
        IRepository<UserPlan> userPlansQuery,
        IRepository<UserPlanAnswer> userPlanAnswersQuery,
        IRepository<QuestionLinked> questionLinksQuery,
        IRepository<PlanQuestion> planQuestionsQuery
        ) : IQueryHandler<GetNextUnansweredQuestionQuery, ServiceResult<GetByIdQuestionDTO>>
    {
        public async Task<ServiceResult<GetByIdQuestionDTO>> Handle(GetNextUnansweredQuestionQuery request, CancellationToken cancellationToken)
        => ServiceResult.Ok(PlanFlowService.GetNextUnansweredQuestion(
                    userPlansQuery.Table,
                    userPlanAnswersQuery.Table,
                    questionLinksQuery.Table,
                    planQuestionsQuery.Table,
                    request.GetNextUnansweredQuestionDTO.PlanId,
                    request.GetNextUnansweredQuestionDTO.UserId));





    }
}
