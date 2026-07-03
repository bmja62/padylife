using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Helpers;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public class GetPlanQuery(long id) : IQuery<ServiceResult<GetPlanDTO>>
    {
        public long Id { get; } = id;
    }

    public class GetPlanQueryHandler(IRepository<Plan> planRepository, IRepository<PlanQuestion> planQuestionRepository, IRepository<QuestionOption> questionOptionRepository, IRepository<QuestionLinked> questionLinkedRepository) : IQueryHandler<GetPlanQuery, ServiceResult<GetPlanDTO>>
    {
        public async Task<ServiceResult<GetPlanDTO>> Handle(GetPlanQuery request, CancellationToken cancellationToken)
        {

            var planExists = await planRepository.Table.AnyAsync(t => t.Id == request.Id);
            if (!planExists)
                return ServiceResult.BadRequest<GetPlanDTO>("Plan not found");

            var query = planRepository.Table.AddQuestionIncludes();
            var plan = await planRepository.Table
            .AddQuestionIncludes()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (plan == null)
                return ServiceResult.BadRequest<GetPlanDTO>("پلن مورد نظر یافت نشد");

            if (plan == null)
                return ServiceResult.BadRequest<GetPlanDTO>("پلن مورد نظر یافت نشد");

            plan.PlanQuestions = await planQuestionRepository.Table.Where(t => t.PlanId == plan.Id).ToListAsync(cancellationToken);

            plan.PlanQuestions.ForEach(item =>
            {
                item.QuestionLinks = questionLinkedRepository.Table.Where(a => a.PlanId == plan.Id && a.PlanQuestionId == item.Id).ToList();
            });
            var result = GetPlanDTO.CreateDefault(
                id: plan.Id,
                title: plan.Title,
                imageUrl: plan.ImageUrl,
                planCategoryId: plan.PlanCategoryId,
                planCategoryName: plan.PlanCategory?.Name,
                description: plan.Description,
                isSignUpPlan: plan.IsSignUpPlan,
                status: plan.Status,
                level: plan.Level,
                price: plan.Price,
                discountPrice: plan.DiscountPrice,
                finalPrice: plan.FinalPrice,
                planQuestions: QuestionTreeService.BuildPlanHierarchy(
                    plan.PlanQuestions,
                    0,
                    15
                    )
            );

            return ServiceResult.Ok<GetPlanDTO>(result);
        }

    }



}

