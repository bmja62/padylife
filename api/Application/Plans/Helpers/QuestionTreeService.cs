using Application.Excersies.DTOs;
using Application.Plans.DTOs;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;

namespace Application.Plans.Helpers
{
    public static class QuestionTreeService
    {
        public static List<PlanQuestionDTO> BuildPlanHierarchy(List<PlanQuestion> planQuestions, int currentDepth = 0, int maxDepth = 5)
        {
            // یافتن سوالات اصلی (IsMain = true)
            var mainQuestions = planQuestions.Where(pq => pq.IsMain).ToList();

            var result = new List<PlanQuestionDTO>();
            var visitedQuestions = new HashSet<long>();

            foreach (var mainQuestion in mainQuestions)
            {
                var questionDto = BuildQuestionNode(mainQuestion, planQuestions, visitedQuestions, currentDepth, maxDepth);
                if (questionDto != null)
                {
                    result.Add(questionDto);
                }
            }

            return result;
        }

        private static PlanQuestionDTO BuildQuestionNode(
            PlanQuestion planQuestion,
            List<PlanQuestion> allPlanQuestions,
            HashSet<long> visitedQuestions,
            int currentDepth = 0,
            int maxDepth = 5)
        {
            if (planQuestion == null ||
                currentDepth >= maxDepth ||
                visitedQuestions.Contains(planQuestion.QuestionId))
                return null;

            visitedQuestions.Add(planQuestion.QuestionId);

            //var questionDto = new PlanQuestionDTO
            //{
            //    Id = planQuestion.Id,
            //    QuestionId = planQuestion.QuestionId,
            //    Text = planQuestion.Question.Text,
            //    IsMain = planQuestion.IsMain,
            //    ReadOnlyQuestionOptions = planQuestion.Question.QuestionOptions.Select(t => ReadOnlyQuestionOptionsDTO.CreateDefault(t.Id,t.Text)).ToList(), 
            //    QuestionOptions = new List<PlanQuestionOptionDTO>()
            //};

            var questionDto = PlanQuestionDTO.CreateDefault(
                planQuestion.Id,
                planQuestion.QuestionId,
                planQuestion.Question.Text,
                planQuestion.Question.DisplayText,
                planQuestion.IsMain,
                new List<PlanQuestionOptionDTO>(),
                planQuestion.Question.QuestionOptions.OrderBy(t => t.Priority).Select(t => ReadOnlyQuestionOptionsDTO.CreateDefault(t.Id, t.Text, t.Priority)).ToList()
                );

            // پردازش گزینه‌های پاسخ از طریق QuestionLinked
            foreach (var questionLink in planQuestion.QuestionLinks)
            {
                var optionDto = new PlanQuestionOptionDTO
                {
                    Id = questionLink.QuestionOptionId,
                    QuestionId = questionLink.QuestionOption.QuestionId,
                    Text = questionLink.QuestionOption.Text,
                    HasValidLinks = questionLink.LinkedPlanQuestionId != null,
                    LinkedExercises = questionLink.ExerciseLinks.OrderBy(t => t.Priority).Select(t => ExerciseDTO.Create(
                        t.ExerciseId,
                        t.Exercise.Title,
                        t.Exercise.ExerciseCategory.Name
                        )).ToList(),
                    LinkedPlanQuestionId = questionLink.LinkedPlanQuestionId,
                };

                // اگر به سوال دیگری لینک شده
                if (questionLink.LinkedPlanQuestionId != null)
                {
                    // یافتن PlanQuestion مربوطه برای سوال لینک شده
                    var linkedPlanQuestion = allPlanQuestions
                        .FirstOrDefault(pq =>
                            pq.Id == questionLink.LinkedPlanQuestionId &&
                            pq.PlanId == questionLink.PlanId);

                    optionDto.LinkedQuestion = BuildQuestionNode(
                        linkedPlanQuestion,
                        allPlanQuestions,
                        new HashSet<long>(visitedQuestions),
                        currentDepth + 1,
                        maxDepth);
                }

                // اگر به تمرین لینک شده
                if (questionLink.ExerciseLinks != null)
                {
                    //optionDto.LinkedExercise = new ExerciseDTO
                    //{
                    //    Id = questionLink.LinkedExercise.Id,
                    //    Title = questionLink.LinkedExercise.Title
                    //};

                    optionDto.LinkedExercises = questionLink.ExerciseLinks.Select(tt => ExerciseDTO.Create(tt.ExerciseId, tt.Exercise.Title, tt.Exercise.ExerciseCategory.Name)).ToList();
                }

                questionDto.QuestionOptions.Add(optionDto);
            }

            return questionDto;
        }
        public static IQueryable<Plan> AddQuestionIncludes(this IQueryable<Plan> query)
        {
            return query
              .Include(p => p.PlanCategory)
        .Include(p => p.PlanQuestions)
            .ThenInclude(pq => pq.Question)
            .ThenInclude(qo => qo.QuestionOptions)
        .Include(p => p.PlanQuestions)
            .ThenInclude(pq => pq.QuestionLinks)
                .ThenInclude(ql => ql.QuestionOption)
        .Include(p => p.PlanQuestions)
            .ThenInclude(pq => pq.QuestionLinks)
                .ThenInclude(ql => ql.LinkedPlanQuestion)
        .Include(p => p.PlanQuestions)
            .ThenInclude(pq => pq.QuestionLinks)
                .ThenInclude(ql => ql.ExerciseLinks)
                    .ThenInclude(ql => ql.Exercise)
                    .ThenInclude(ql => ql.ExerciseCategory);
        }
    }
}

