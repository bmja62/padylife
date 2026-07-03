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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.Queries
{
    public record GetPlanExerciseAnswersQuery(GetPlanExerciseAnswersRequest Dto)
        : IQuery<ServiceResult<GlobalGridResult<PlanExerciseAnswersItem>>>;

    public class GetPlanExerciseAnswersQueryHandler(
        IRepository<UserPlan> _userPlanRepo,
        IRepository<UserPlanExcersiesAnswer> _userPlanExerciseAnswersRepo,
        IHttpContextAccessor httpContextAccessor
    ) : IQueryHandler<GetPlanExerciseAnswersQuery, ServiceResult<GlobalGridResult<PlanExerciseAnswersItem>>>
    {
        public async Task<ServiceResult<GlobalGridResult<PlanExerciseAnswersItem>>> Handle(GetPlanExerciseAnswersQuery request, CancellationToken ct)
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
                

                // فیلتر وضعیت تکمیل
                if (request.Dto.OnlyCompleted == true)
                    q = q.Where(up => up.IsCompleted);

                // فیلتر جستجو در نام کاربر
                if (!string.IsNullOrWhiteSpace(request.Dto.Search))
                    q = q.Where(up => up.User.FullName.Contains(request.Dto.Search!) || up.User.UserName.Contains(request.Dto.Search!));

                // فیلتر تاریخ شروع
                if (request.Dto.StartDateFrom.HasValue)
                    q = q.Where(up => up.StartDate >= request.Dto.StartDateFrom.Value);

                if (request.Dto.StartDateTo.HasValue)
                    q = q.Where(up => up.StartDate <= request.Dto.StartDateTo.Value);

                // فیلتر تاریخ پایان
                if (request.Dto.EndDateFrom.HasValue)
                    q = q.Where(up => up.EndDate >= request.Dto.EndDateFrom.Value);

                if (request.Dto.EndDateTo.HasValue)
                    q = q.Where(up => up.EndDate <= request.Dto.EndDateTo.Value);

                // فیلتر بر اساس تمرین خاص
                if (request.Dto.ExerciseId.HasValue)
                {
                    q = q.Where(up => up.Answers.Any(a =>
                        _userPlanExerciseAnswersRepo.TableNoTracking
                            .Any(ea => ea.UserPlanId == up.Id && ea.ExcersieId == request.Dto.ExerciseId.Value)));
                }

                // فیلتر بر اساس مرحله خاص
                if (request.Dto.StepId.HasValue)
                {
                    q = q.Where(up => up.Answers.Any(a =>
                        _userPlanExerciseAnswersRepo.TableNoTracking
                            .Any(ea => ea.UserPlanId == up.Id && ea.StepId == request.Dto.StepId.Value)));
                }

                // فیلتر بر اساس نوع پاسخ (دارای متن، تصویر یا گزینه)
                if (!string.IsNullOrWhiteSpace(request.Dto.AnswerType))
                {
                    switch (request.Dto.AnswerType.ToLower())
                    {
                        case "text":
                            q = q.Where(up => up.Answers.Any(a =>
                                _userPlanExerciseAnswersRepo.TableNoTracking
                                    .Any(ea => ea.UserPlanId == up.Id && !string.IsNullOrEmpty(ea.Text))));
                            break;
                        case "image":
                            q = q.Where(up => up.Answers.Any(a =>
                                _userPlanExerciseAnswersRepo.TableNoTracking
                                    .Any(ea => ea.UserPlanId == up.Id && !string.IsNullOrEmpty(ea.ImageUrl))));
                            break;
                        case "choice":
                            q = q.Where(up => up.Answers.Any(a =>
                                _userPlanExerciseAnswersRepo.TableNoTracking
                                    .Any(ea => ea.UserPlanId == up.Id && ea.SelectedStepOptionId.HasValue)));
                            break;
                        case "multiple-choice":
                            q = q.Where(up => up.Answers.Any(a =>
                                _userPlanExerciseAnswersRepo.TableNoTracking
                                    .Any(ea => ea.UserPlanId == up.Id && ea.SelectedChoices.Any())));
                            break;
                    }
                }

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
                        up.EndDate,
                        up.IsCompleted,
                        up.IsSignUpPlan
                    })
                    .ToListAsync(ct);

                var userPlanIds = userPlans.Select(up => up.Id).ToList();

                // Get exercise answers with related data
                var exerciseAnswers = await _userPlanExerciseAnswersRepo.TableNoTracking
                    .Where(ea => userPlanIds.Contains(ea.UserPlanId))
                    .Select(ea => new
                    {
                        ea.Id,
                        ea.UserPlanId,
                        ea.ExcersieId,
                        ea.StepId,
                        ea.SelectedStepOptionId,
                        ea.Text,
                        ea.ImageUrl,
                        ExerciseName = ea.Exercise.Title, // فرض بر وجود فیلد Name در Exercise
                        StepName = ea.Step.Name, // فرض بر وجود فیلد Title در Step
                        SelectedOptionText = ea.SelectedStepOption != null ? ea.SelectedStepOption.Title : null,
                        SelectedChoices = ea.SelectedChoices.Select(sc => new
                        {
                            sc.OptionChoiceId,
                            ChoiceText = sc.Choice.Text,
                            ChoiceIsCorrect = sc.Choice.IsCorrect,
                            ChoiceFeedback = sc.Choice.Feedback
                        }).ToList()
                    })
                    .ToListAsync(ct);

                var answersByUserPlan = exerciseAnswers
                    .GroupBy(ea => ea.UserPlanId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var data = userPlans.Select(up => new PlanExerciseAnswersItem(
                    up.Id,
                    up.UserId,
                    string.IsNullOrEmpty(up.UserFullName) ? up.UserUserName : up.UserFullName,
                    up.StartDate,
                    up.EndDate,
                    up.IsCompleted,
                    up.IsSignUpPlan,
                    answersByUserPlan.ContainsKey(up.Id)
                        ? answersByUserPlan[up.Id]
                            .Select(ea => new ExerciseAnswerDto(
                                ea.Id,
                                ea.ExcersieId,
                                ea.ExerciseName,
                                ea.StepId,
                                ea.StepName,
                                ea.SelectedStepOptionId,
                                ea.SelectedOptionText,
                                ea.Text,
                                ea.ImageUrl,
                                ea.SelectedChoices.Select(sc => new SelectedChoiceDto(
                                    sc.OptionChoiceId,
                                    sc.ChoiceText,
                                    sc.ChoiceIsCorrect,
                                    sc.ChoiceFeedback
                                )).ToList()
                            ))
                            .OrderBy(x => x.ExerciseId)
                            .ThenBy(x => x.StepId)
                            .ToList()
                        : new List<ExerciseAnswerDto>()
                )).ToList();

                return ServiceResult.Ok(new GlobalGridResult<PlanExerciseAnswersItem>
                {
                    Data = data,
                    TotalCount = total
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.BadRequest<GlobalGridResult<PlanExerciseAnswersItem>>(ex.Message);
            }
        }
    }
}
