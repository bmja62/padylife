using Application.Excersies.DTOs;
using Application.Plans.DTOs;
using Application.Questions.DTOs;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Plans.Helpers;

public static class PlanFlowService
{
    public static GetPlanFlowDTO GetNextStep(
        IQueryable<QuestionLinked> questionLinksQuery,
        IQueryable<PlanQuestion> planQuestionQuery,
        long planId,
        long currentPlanQuestionId,
        long selectedOptionId)
    {
        // یافتن لینک مربوطه برای این پلن و گزینه انتخابی
        var selectedLink = questionLinksQuery
            .Include(t => t.ExerciseLinks)
            .ThenInclude(t => t.Exercise)
            .ThenInclude(t => t.ExerciseSteps)
            .ThenInclude(t => t.Step)
            .Include(t => t.ExerciseLinks)
            .ThenInclude(t => t.Exercise)
            .ThenInclude(t => t.ExerciseCategory)
            .FirstOrDefault(ql =>
                ql.PlanId == planId &&
                ql.PlanQuestionId == currentPlanQuestionId &&
                ql.QuestionOptionId == selectedOptionId);

        if (selectedLink?.ExerciseLinks != null && selectedLink.ExerciseLinks.Count > 0)
            return GetPlanFlowDTO.CreateExercise(selectedLink
                .ExerciseLinks
                .Select(t =>
                    GetAllExcersieDTO.CreateDefalt(
                        t.Exercise.Id,
                        t.Exercise.Title,
                        t.Exercise.ImageUrl,
                        t.Exercise.ExerciseCategoryId,
                        t.Exercise.ExerciseCategory.Name,
                        t.Exercise.DocumentLink,
                        t.Exercise.CreatedAt,
                        t.Exercise.ExerciseEstimate,
                        t.Exercise.ExerciseGoal,
                        t.Exercise.ExerciseCount,
                        t.Exercise.ExerciseType,
                        t.Exercise.UpdatedAt,
                        t.Exercise.PracticeMethod,
                        t.QuestionLinkedId,
                        t.Exercise.ExerciseSteps.Select(tt =>
                            ExerciseStepsDTO.CreateDefault(
                                tt.StepId,
                                tt.ExerciseId,
                                tt.Step.Name,
                                DateTime.Now)).ToList()
                    )
                ).ToList());

        if (selectedLink?.LinkedPlanQuestionId != null)
            return planQuestionQuery
                .Where(q => q.Id == selectedLink.LinkedPlanQuestionId)
                .Select(t => GetPlanFlowDTO.CreateQuestion(
                    GetByIdQuestionDTO.CreateDefault(
                        t.Id,
                        t.Question.QuestionCategoryId,
                        t.Question.QuestionCategory.Name,
                        t.Question.Text,
                        t.Question.DisplayText,
                        t.Question.QuestionOptions.Select(tt =>
                            GetAllQuestionQuestionOptionDTO.CreateDefault(
                                tt.Id,
                                tt.QuestionId,
                                tt.Text,
                                tt.Priority
                                )).ToList()
                    )
                ))
                .FirstOrDefault();
        return null;
    }

    public static GetByIdQuestionDTO GetNextUnansweredQuestion(
     IQueryable<UserPlan> userPlansQuery,
     IQueryable<UserPlanAnswer> userPlanAnswersQuery,
     IQueryable<QuestionLinked> questionLinksQuery,
     IQueryable<PlanQuestion> planQuestionsQuery,
     long planId,
     long userId)
    {
        // یافتن پلن کاربر
        var userPlan = userPlansQuery
            .FirstOrDefault(up => up.PlanId == planId && up.UserId == userId);

        if (userPlan == null) return null;

        // یافتن تمام پاسخ‌های کاربر برای این پلن
        var answeredQuestions = userPlanAnswersQuery
            .Where(upa => upa.UserPlanId == userPlan.Id)
            .Select(upa => upa.PlanQuestionId)
            .ToList();

        long? nextQuestionId = null;

        // اگر کاربر هیچ پاسخی نداده، اولین سوال اصلی را برگردان
        if (!answeredQuestions.Any())
        {
            nextQuestionId = questionLinksQuery
                .Where(ql => ql.PlanId == planId && ql.PlanQuestion.IsMain)
                .Select(ql => ql.PlanQuestionId)
                .FirstOrDefault();
        }
        else
        {
            // یافتن آخرین سوال پاسخ داده شده
            var lastAnsweredQuestion = answeredQuestions.Last();

            // یافتن لینک مربوط به آخرین پاسخ کاربر
            var lastAnswer = userPlanAnswersQuery
                .Where(upa => upa.UserPlanId == userPlan.Id && upa.PlanQuestionId == lastAnsweredQuestion)
                .OrderByDescending(upa => upa.Id)
                .FirstOrDefault();

            if (lastAnswer != null)
            {
                // یافتن گزینه انتخابی کاربر
                var selectedOptionId = lastAnswer.SelectedQuestionOptionId;

                // یافتن سوال بعدی بر اساس پاسخ کاربر
                var nextQuestionLink = questionLinksQuery
                    .FirstOrDefault(ql =>
                        ql.PlanId == planId &&
                        ql.PlanQuestionId == lastAnsweredQuestion &&
                        ql.QuestionOptionId == selectedOptionId);

                // اگر سوال بعدی وجود دارد و کاربر هنوز به آن پاسخ نداده
                if (nextQuestionLink?.LinkedPlanQuestionId != null &&
                    !answeredQuestions.Contains(nextQuestionLink.LinkedPlanQuestionId.Value))
                {
                    nextQuestionId = nextQuestionLink.LinkedPlanQuestionId;
                }
            }
        }

        if (!nextQuestionId.HasValue) return null;

        // یافتن اطلاعات کامل سوال
        var nextQuestion = planQuestionsQuery
            .Include(pq => pq.Question)
                .ThenInclude(q => q.QuestionCategory)
            .Include(pq => pq.Question)
                .ThenInclude(q => q.QuestionOptions)
            .FirstOrDefault(pq => pq.Id == nextQuestionId.Value);

        if (nextQuestion == null) return null;

        // تبدیل به DTO
        return GetByIdQuestionDTO.CreateDefault(
            nextQuestion.Id,
            nextQuestion.Question.QuestionCategoryId,
            nextQuestion.Question.QuestionCategory.Name,
            nextQuestion.Question.Text,
            nextQuestion.Question.DisplayText,
            nextQuestion.Question.QuestionOptions
                .OrderBy(o => o.Priority)
                .Select(o => GetAllQuestionQuestionOptionDTO.CreateDefault(
                    o.Id,
                    o.QuestionId,
                    o.Text,
                    o.Priority))
                .ToList()
        );
    }

    public static async Task<List<UserPlanStatusDTO>> GetUserPlansStatus(
    IQueryable<UserPlan> userPlansQuery,
    IQueryable<UserPlanAnswer> userPlanAnswersQuery,
    IQueryable<QuestionLinked> questionLinksQuery,
    IQueryable<PlanQuestion> planQuestionsQuery,
    IQueryable<Plan> plansQuery,
    long userId,
    long? planId)
    {
        var result = new List<UserPlanStatusDTO>();

        // دریافت تمام پلن‌های کاربر
        var userPlans = await userPlansQuery
            .Where(up => up.UserId == userId)
            .Include(up => up.Plan)
            .ToListAsync();

        if (planId.HasValue && planId > 0)
            userPlans = userPlans.Where(t => t.PlanId == planId.Value).ToList();

        foreach (var userPlan in userPlans)
        {
            // دریافت پاسخ‌های کاربر با گروه‌بندی و انتخاب آخرین پاسخ برای هر سوال
            var latestAnswersQuery = userPlanAnswersQuery
                .Where(upa => upa.UserPlanId == userPlan.Id)
                .Include(upa => upa.PlanQuestion)
                    .ThenInclude(pq => pq.Question)
                .Include(upa => upa.SelectedQuestionOption)
                .GroupBy(upa => upa.PlanQuestionId)
                .Select(g => g.OrderByDescending(upa => upa.CreatedAt).FirstOrDefault());

            var latestAnswers = await latestAnswersQuery.ToListAsync();

            var status = new UserPlanStatusDTO
            {
                UserPlanId = userPlan.Id,
                PlanId = userPlan.PlanId,
                PlanName = userPlan.Plan.Title,
                ImageUrl = userPlan.Plan.ImageUrl,
                PlanLevel = userPlan.Plan.Level,
                AnsweredQuestions = latestAnswers
                    .Where(a => a != null)
                    .Select(a => new UserAnsweredQuestionDTO
                    {
                        UserPlanId = a.UserPlanId,
                        PlanQuestionId = a.PlanQuestionId,
                        QuestionText = a.PlanQuestion.Question.Text,
                        SelectedOptionId = a.SelectedQuestionOptionId,
                        SelectedOptionText = a.SelectedQuestionOption.Text,
                        AnswerDate = a.CreatedAt,
                        PlanId = userPlan.PlanId,
                        PlanTitle = userPlan.Plan.Title
                    }).ToList(),
                LastAnswerExercises = new List<ExerciseDTO>(),
                NextUnansweredQuestion = GetNextUnansweredQuestion(userPlansQuery, userPlanAnswersQuery, questionLinksQuery, planQuestionsQuery, userPlan.PlanId, userId)
            };

            // بررسی آخرین پاسخ و تمرینات مرتبط
            if (latestAnswers.Any(a => a != null))
            {
                var lastAnswer = latestAnswers
                    .Where(a => a != null)
                    .OrderByDescending(a => a.CreatedAt)
                    .First();

                var lastAnswerLink = await questionLinksQuery
                    .Where(ql => ql.PlanId == userPlan.PlanId &&
                                ql.PlanQuestionId == lastAnswer.PlanQuestionId &&
                                ql.QuestionOptionId == lastAnswer.SelectedQuestionOptionId)
                    .Include(ql => ql.ExerciseLinks)
                        .ThenInclude(el => el.Exercise)
                            .ThenInclude(e => e.ExerciseCategory)
                    .FirstOrDefaultAsync();

                if (lastAnswerLink?.ExerciseLinks != null)
                {
                    status.LastAnswerExercises = lastAnswerLink.ExerciseLinks
                        .Select(el => new ExerciseDTO
                        {
                            Id = el.ExerciseId,
                            Title = el.Exercise.Title,
                            ImageUrl = el.Exercise.ImageUrl,
                            CategoryName = el.Exercise.ExerciseCategory.Name
                        })
                        .ToList();
                }
            }

            result.Add(status);
        }

        return result;
    }

    public static bool HasUserAnySignUpPlan
        (
    IQueryable<UserPlan> allUserPlans,
    long userId,
    long planId)
    => allUserPlans.Any(t => t.IsSignUpPlan && t.UserId == userId && t.PlanId == planId);

}

