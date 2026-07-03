using Application.Cqrs.Queris;
using Application.Excersies.DTOs;
using Data.Contracts;
using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Excersies.Queries
{
    public class GetExcersieQuery : IQuery<ServiceResult<GetAllExcersieDTO>>
    {
        public GetExcersieQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class GetExcersieQueryHandler(IRepository<Exercise> exerciseRepository) : IQueryHandler<GetExcersieQuery, ServiceResult<GetAllExcersieDTO>>
    {
        public async Task<ServiceResult<GetAllExcersieDTO>> Handle(GetExcersieQuery request, CancellationToken cancellationToken)
        => ServiceResult.Ok(await exerciseRepository.Table.Where(t => !t.IsDeleted && t.Id == request.Id).Select(t => GetAllExcersieDTO.CreateDefalt(
                    t.Id,
                    t.Title,
                    t.ImageUrl,
                    t.ExerciseCategoryId,
                    t.ExerciseCategory.Name,
                    t.DocumentLink,
                    t.CreatedAt,
                    t.ExerciseEstimate,
                    t.ExerciseGoal,
                    t.ExerciseCount,
                    t.ExerciseType,
                    t.UpdatedAt,
                    t.PracticeMethod,
                    0,
                    t.ExerciseSteps.Select(tt => ExerciseStepsDTO.CreateDefault(
                        tt.StepId,
                        tt.ExerciseId,
                        tt.Step.Name,
                        tt.Step.CreatedAt
                        )).ToList()
                    )).FirstOrDefaultAsync());
    }
}
