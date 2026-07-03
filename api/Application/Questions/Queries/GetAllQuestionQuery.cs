using Application.Cqrs.Queris;
using Application.Questions.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Queries
{
    public class GetAllQuestionQuery(GetAllFilterDTO input) : IQuery<ServiceResult<GlobalGridResult<GetAllQuestionDTO>>>
    {
        public GetAllFilterDTO Input { get; } = input;
    }

    public class GetAllQuestionQueryHandler(IRepository<Question> questionRepository,IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetAllQuestionQuery, ServiceResult<GlobalGridResult<GetAllQuestionDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetAllQuestionDTO>>> Handle(GetAllQuestionQuery request, CancellationToken cancellationToken)
        {
            var query = questionRepository.Table;

            if (!string.IsNullOrEmpty(request.Input.Search))
            {
                query = query.Where(t => t.Text.Contains(request.Input.Search));
            }

            var identity = httpContextAccessor.HttpContext.User.Identity;
            var creator = identity.GetUserId<long>();

            if (!identity.IsAdmin())
                query = query.Where(t => t.CreatedByUserId == creator);

            var data = await query.Select(t => new GetAllQuestionDTO
            {
                Id = t.Id,
                QuestionCategoryId = t.QuestionCategoryId,
                QuestionCategoryName = t.QuestionCategory.Name,
                Text = t.Text,
                DisplayText = t.DisplayText,
                Options = t.QuestionOptions.Select(tt =>
                GetAllQuestionQuestionOptionDTO.CreateDefault(
                    tt.Id,
                    tt.QuestionId,
                    tt.Text,
                    tt.Priority
                    )).ToList()
            }).Skip(request.Input.Skip).Take(request.Input.Take).ToListAsync();

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<GetAllQuestionDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
