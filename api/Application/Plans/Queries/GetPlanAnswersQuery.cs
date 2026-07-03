using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetPlanAnswersQuery(GetPlanAnswersRequest Dto)
     : IQuery<ServiceResult<GlobalGridResult<PlanAnswersItem>>>;

    public class GetPlanAnswersQueryHandler(
    IRepository<UserPlan> _userPlanRepo,
    IRepository<UserPlanAnswer> _UserPlanAnswersRepo,
    IHttpContextAccessor httpContextAccessor
) : IQueryHandler<GetPlanAnswersQuery, ServiceResult<GlobalGridResult<PlanAnswersItem>>>
    {
        public async Task<ServiceResult<GlobalGridResult<PlanAnswersItem>>> Handle(GetPlanAnswersQuery request, CancellationToken ct)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var isAdmin = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.Name);

            try
            {
                // Build the base query
                var q = _userPlanRepo.TableNoTracking
                    .Where(up => up.PlanId == request.Dto.PlanId && up.Answers.Any());
                if (request.Dto.ForAdmin.HasValue && request.Dto.ForAdmin.Value) 
                {
                    if (!isAdmin)
                    {
                        q = q.Where(up => up.Experts.Any(e => e.ExpertId == userId));
                    }
                }
                

                if (request.Dto.OnlyCompleted == true)
                    q = q.Where(up => up.IsCompleted);

                if (!string.IsNullOrWhiteSpace(request.Dto.Search))
                    q = q.Where(up => up.User.FullName.Contains(request.Dto.Search!) || up.User.UserName.Contains(request.Dto.Search!));

                // Get total count first
                var total = await q.CountAsync(ct);

                // Step 1: Get the basic UserPlan data without complex projections
                var userPlans = await q
                    .OrderByDescending(up => up.StartDate)
                    .Skip(request.Dto.Skip)
                    .Take(request.Dto.Take)
                    .Select(up => new
                    {
                        up.Id,
                        up.UserId,
                        UserFullName = up.User.FullName,
                        UserUserName = up.User.UserName,
                        up.StartDate,
                        up.IsCompleted
                    })
                    .ToListAsync(ct);

                var userPlanIds = userPlans.Select(up => up.Id).ToList();

                var answers = await _UserPlanAnswersRepo.TableNoTracking
                    .Where(a => userPlanIds.Contains(a.UserPlanId))
                    .Select(a => new
                    {
                        a.UserPlanId,
                        a.PlanQuestionId,
                        PlanQuestionIsMain = a.PlanQuestion.IsMain,
                        a.PlanQuestion.QuestionId,
                        QuestionText = a.PlanQuestion.Question.Text,
                        a.SelectedQuestionOptionId,
                        SelectedOptionText = a.SelectedQuestionOption != null
                            ? a.SelectedQuestionOption.Text
                            : null
                    })
                    .ToListAsync(ct);

                var answersByUserPlan = answers
                    .GroupBy(a => a.UserPlanId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var data = userPlans.Select(up => new PlanAnswersItem(
                    up.Id,
                    up.UserId,
                    string.IsNullOrEmpty(up.UserFullName) ? up.UserUserName : up.UserFullName,
                    up.StartDate,
                    up.IsCompleted,
                    answersByUserPlan.ContainsKey(up.Id)
                        ? answersByUserPlan[up.Id]
                            .Select(a => new AnswerDto(
                                a.PlanQuestionId,
                                a.PlanQuestionIsMain,
                                a.QuestionId,
                                a.QuestionText,
                                a.SelectedQuestionOptionId,
                                a.SelectedOptionText
                            ))
                            .OrderBy(x => x.IsMain)
                            .ThenBy(x => x.PlanQuestionId)
                            .ToList()
                        : new List<AnswerDto>()
                )).ToList();

                return ServiceResult.Ok(new GlobalGridResult<PlanAnswersItem>
                {
                    Data = data,
                    TotalCount = total
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.BadRequest<GlobalGridResult<PlanAnswersItem>>(ex.Message);
            }
        }
    }

}
