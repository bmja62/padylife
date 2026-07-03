using Application.Cqrs.Queris;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Queries
{
    public class GetByIdQuestionQuery : IQuery<ServiceResult<GetByIdQuestionDTO>>
    {
        public GetByIdQuestionQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class GetByIdQuestionQueryHandler(IRepository<Question> questionRepository) : IQueryHandler<GetByIdQuestionQuery, ServiceResult<GetByIdQuestionDTO>>
    {
        public async Task<ServiceResult<GetByIdQuestionDTO>> Handle(GetByIdQuestionQuery request, CancellationToken cancellationToken)
        {
            var questionInDb = await questionRepository.Table.Where(t => t.Id == request.Id).Select(t =>
            GetByIdQuestionDTO.CreateDefault
            (
                t.Id,
                t.QuestionCategoryId,
                t.QuestionCategory.Name,
                t.Text,
                t.DisplayText,
                t.QuestionOptions.Select(tt =>
                GetAllQuestionQuestionOptionDTO.CreateDefault(
                    tt.Id,
                    tt.QuestionId,
                    tt.Text,
                    tt.Priority
                    )).ToList()
                )
            ).FirstOrDefaultAsync();
            return ServiceResult.Ok(questionInDb);

        }
    }
}
