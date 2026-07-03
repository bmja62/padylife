using Application.Questions.DTOs;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PadyLife.Api.Models.Users.Extentions
{
    internal static class UserRepositoryExtensions
    {
        public static IQueryable<UserDTO> SelectUsers(
       this IQueryable<User> input,
       IQueryable<UserPlan> allUserPlans = null,
       IQueryable<UserPlanAnswer> allUserPlanAnswers = null,
       IQueryable<QuestionLinked> allQuestionLinks = null,
       IQueryable<PlanQuestion> allPlanQuestions = null,
       bool includeSignUpInfo = false)
        {
            return includeSignUpInfo ?
                input.Select(SelectUsersExWithSignUpInfo(
                    allUserPlans,
                    allUserPlanAnswers,
                    allQuestionLinks,
                    allPlanQuestions
                    )) :
                input.Select(SelectUsersEx());
        }

        public static Expression<Func<User, UserDTO>> SelectUsersEx() =>
            t => UserDTO.Create(
                t.Id,
                t.FullName,
                t.IsActive,
                t.PhoneNumber,
                t.Gender,
                t.Email,
                t.EmailConfirmed,
                t.UserRoles.Select(tt => RoleDTO.Create(tt.Role.Name, tt.Role.Description)).ToArray(),
                t.UserName,
                t.UserPlans.Select(tt => UserPlanDTO.Create(tt.Id, tt.PlanId, tt.UserId)).ToList() ?? new List<UserPlanDTO>(),
                t.MaritalStatus,
                t.JobTitle,
                t.Wight,
                t.Hight,
                t.Birthdate,
                t.InstagramId,
                t.Addresses.Select(t => t.GetFullAddress() ?? "-").ToList(),
                t.Age,
                t.ProfileImage,
                t.UserMedals.OrderByDescending(t => t.AwardedAt).Select(t => t.Medal.IconUrl).FirstOrDefault()
            );


        public static Expression<Func<User, UserDTO>> SelectUsersExWithSignUpInfo(
            IQueryable<UserPlan> allUserPlans,
            IQueryable<UserPlanAnswer> allUserPlanAnswers,
            IQueryable<QuestionLinked> allQuestionLinks,
            IQueryable<PlanQuestion> allPlanQuestions)
        {

            return t => new UserDTO
            {
                Id = t.Id,
                FullName = t.FullName,
                IsActive = t.IsActive,
                PhoneNumber = t.PhoneNumber,
                Gender = t.Gender,
                Email = t.Email,
                EmailConfirmed = t.EmailConfirmed,
                Roles = t.UserRoles.Select(tt => RoleDTO.Create(tt.Role.Name, tt.Role.Description)).ToArray(),
                UserName = t.UserName,
                UserPlans = t.UserPlans.Select(tt => UserPlanDTO.Create(tt.Id, tt.PlanId, tt.UserId)).ToList() ?? new List<UserPlanDTO>(),
                Accompanied = allUserPlans
            .SelectMany(up => up.Companions)
            .Where(c => c.CompanionUserId == t.Id)
            .Count(),

                // Enhanced version with summary and details:
                AccompaniedSummary = allUserPlans
            .SelectMany(up => up.Companions)
            .Where(c => c.CompanionUserId == t.Id)
            .GroupBy(c => c.UserPlan.UserId)
            .Select(g => new AccompaniedSummaryDTO
            {
                PlanOwnerUserId = g.Key,
                Count = g.Count(),
                PlanOwnerName = g.First().UserPlan.User.FullName
            }).ToList(),

                TotalUniquePlanOwners = allUserPlans
            .SelectMany(up => up.Companions)
            .Where(c => c.CompanionUserId == t.Id)
            .Select(c => c.UserPlan.UserId)
            .Distinct()
            .Count(),

                AccompaniedDetails = allUserPlans
            .SelectMany(up => up.Companions)
            .Where(c => c.CompanionUserId == t.Id)
            .Select(c => new AccompaniedDetailDTO
            {
                UserPlanId = c.UserPlanId,
                PlanId = c.UserPlan.PlanId,
                PlanOwnerUserId = c.UserPlan.UserId,
                PlanOwnerName = c.UserPlan.User.FullName,
                CompanionSince = c.CreatedAt,
                PlanInfo = new UserPlanBasicDTO
                {
                    PlanName = c.UserPlan.Plan.Title,
                    StartDate = c.UserPlan.StartDate
                }
            }).OrderByDescending(d => d.CompanionSince)
            .ToList(),
                Supported = allUserPlans
            .Join(allUserPlans.SelectMany(up => up.Companions),
                up => up.Id,
                c => c.UserPlanId,
                (up, c) => new { UserPlan = up, Companion = c })
            .Where(j => t.UserPlans.Select(up => up.Id).Contains(j.Companion.UserPlanId))
            .Count(),
                CompanionSummary = allUserPlans
            .Where(up => up.UserId == t.Id)
            .SelectMany(up => up.Companions)
            .GroupBy(c => c.CompanionUserId)
            .Select(g => new CompanionSummaryDTO
            {
                UserId = g.Key,
                Count = g.Count()
            }).ToList(),

                // Total unique companions
                TotalUniqueCompanions = allUserPlans
            .Where(up => up.UserId == t.Id)
            .SelectMany(up => up.Companions)
            .Select(c => c.CompanionUserId)
            .Distinct()
            .Count(),

                // Detailed companion records
                CompanionDetails = allUserPlans
            .Where(up => up.UserId == t.Id)
            .SelectMany(up => up.Companions.Select(c => new CompanionDetailDTO
            {
                UserPlanId = up.Id,
                PlanId = up.PlanId,
                PlanOwnerUserId = up.UserId,
                CompanionUserId = c.CompanionUserId,
                CompanionAddedDate = c.CreatedAt
            })).ToList(),
                SignUpPlanInfo = (
                    from up in allUserPlans
                    where up.UserId == t.Id && up.IsSignUpPlan
                    orderby up.Id
                    select new
                    {
                        PlanId = up.PlanId,
                        UserPlanId = up.Id,
                        HasAnySignUp = true,

                        LastAnswered = (
                            from ua in allUserPlanAnswers
                            where ua.UserPlanId == up.Id
                            orderby ua.CreatedAt descending
                            select new
                            {
                                ua.PlanQuestionId,
                                ua.SelectedQuestionOptionId
                            }
                        ).FirstOrDefault(),

                        AnsweredQuestions = (
                            from ua in allUserPlanAnswers
                            where ua.UserPlanId == up.Id
                            select ua.PlanQuestionId
                            ).ToList()
                    }).Take(1).FirstOrDefault() != null ? (
                    from up in allUserPlans
                    where up.UserId == t.Id && up.IsSignUpPlan
                    orderby up.Id
                    select new
                    {
                        PlanId = up.PlanId,
                        UserPlanId = up.Id,
                        HasAnySignUp = true,

                        LastAnswered = (
                            from ua in allUserPlanAnswers
                            where ua.UserPlanId == up.Id
                            orderby ua.CreatedAt descending
                            select new
                            {
                                ua.PlanQuestionId,
                                ua.SelectedQuestionOptionId
                            }
                        ).FirstOrDefault(),

                        AnsweredQuestions = (
                            from ua in allUserPlanAnswers
                            where ua.UserPlanId == up.Id
                            select ua.PlanQuestionId
                            ).ToList()
                    }).Take(1)
                    .Select(info => UserSignUpPlanInfoDTO.Create(
                        true,
                        (
                            info.LastAnswered == null
                            ? (
                                // اگر پاسخی نداشته، سؤال اصلی رو برگردون
                                (from ql in allQuestionLinks
                                 where ql.PlanId == info.PlanId && ql.PlanQuestion.IsMain
                                 select GetByIdQuestionDTO.CreateDefault(
                                     ql.PlanQuestion.Id,
                                     ql.PlanQuestion.Question.QuestionCategoryId,
                                     ql.PlanQuestion.Question.QuestionCategory.Name,
                                     ql.PlanQuestion.Question.Text,
                                     ql.PlanQuestion.Question.DisplayText,
                                     ql.PlanQuestion.Question.QuestionOptions
                                        .OrderBy(o => o.Priority)
                                        .Select(o => GetAllQuestionQuestionOptionDTO.CreateDefault(
                                            o.Id,
                                            o.QuestionId,
                                            o.Text,
                                            o.Priority
                                        )).ToList()
                                 )).FirstOrDefault()
                            )
                            : (
                                // در غیر این صورت سؤال بعدی بر اساس پاسخ قبلی
                                (from ql in allQuestionLinks
                                 where ql.PlanId == info.PlanId
                                     && ql.PlanQuestionId == info.LastAnswered.PlanQuestionId
                                     && ql.LinkedPlanQuestionId.HasValue
                                     && ql.QuestionOptionId == info.LastAnswered.SelectedQuestionOptionId
                                     && !info.AnsweredQuestions.Contains(ql.LinkedPlanQuestionId.Value)
                                 select GetByIdQuestionDTO.CreateDefault(
                                     ql.LinkedPlanQuestion.Id,
                                     ql.LinkedPlanQuestion.Question.QuestionCategoryId,
                                     ql.LinkedPlanQuestion.Question.QuestionCategory.Name,
                                     ql.LinkedPlanQuestion.Question.Text,
                                     ql.LinkedPlanQuestion.Question.DisplayText,
                                     ql.LinkedPlanQuestion.Question.QuestionOptions
                                        .OrderBy(o => o.Priority)
                                        .Select(o => GetAllQuestionQuestionOptionDTO.CreateDefault(
                                            o.Id,
                                            o.QuestionId,
                                            o.Text,
                                            o.Priority
                                        )).ToList()
                                 )).FirstOrDefault()
                            )
                        )
                    )).FirstOrDefault() : new UserSignUpPlanInfoDTO(),
                MaritalStatus = t.MaritalStatus,
                JobTitle = t.JobTitle,
                Wight = t.Wight,
                Hight = t.Hight,
                Birthdate = t.Birthdate,
                InstagramId = t.InstagramId,
                Addresses = t.Addresses.Select(t => t.GetFullAddress() ?? "-").ToList(),
                Age = t.Age,
                ProfileImage = t.ProfileImage,
                IconUrl = t.UserMedals.OrderByDescending(t => t.AwardedAt).Select(t => t.Medal.IconUrl).FirstOrDefault()
            };
        }
        public static IQueryable<User> ApplyDefaultInclude(this IQueryable<User> query)
        {
            return query
                .Include(u => u.Addresses)
                    .ThenInclude(a => a.Country)
                .Include(u => u.Addresses)
                    .ThenInclude(a => a.Province)
                .Include(u => u.Addresses)
                    .ThenInclude(a => a.City)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserMedals)
                    .ThenInclude(um => um.Medal)
                ;

        }
    }




}
