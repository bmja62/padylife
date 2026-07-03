using Application.Cqrs.Queris;
using Application.Excersies.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Excersies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
namespace Application.Excersies.Queries
{
    public class GetAllExcersieQuery : IQuery<ServiceResult<GlobalGridResult<GetAllExcersieDTO>>>
    {
        public GetAllExcersieQuery(GlobalGrid input)
        {
            Input = input;
        }

        public GlobalGrid Input { get; }
    }

    public class GetAllExcersieQueryHandler(IRepository<Exercise> exerciseRepository,IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetAllExcersieQuery, ServiceResult<GlobalGridResult<GetAllExcersieDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetAllExcersieDTO>>> Handle(GetAllExcersieQuery request, CancellationToken cancellationToken)
        {
            var query = exerciseRepository.Table.Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(request.Input.Search))
                query = query.Where(t => t.Title.Contains(request.Input.Search));

            query = query.OrderByDescending(t => t.CreatedAt).AsQueryable();

            var identity = httpContextAccessor.HttpContext.User.Identity;
            var creator = identity.GetUserId<long>();

            if (!identity.IsAdmin())
                query = query.Where(t => t.CreatedByUserId == creator);


            var data = await query
                .Select(t => GetAllExcersieDTO.CreateDefalt(
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
                    ))
                .Skip((request.Input.PageNumber.Value - 1) * request.Input.Count.Value)
                .Take(request.Input.Count.Value)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<GetAllExcersieDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
