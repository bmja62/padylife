using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.GridResults;
using Data.Contracts;
using Entities.Common;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebFramework.Api
{
    [ApiVersion("1")]
    public class CrudController<TDto, TSelectDto, TEntity, TKey> : BaseController
        where TDto : BaseDto<TDto, TEntity, TKey>, new()
        where TSelectDto : BaseDto<TSelectDto, TEntity, TKey>, new()
        where TEntity : class, IEntity<TKey>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudController(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<GlobalGridResult<TSelectDto>>> Get(int pageNumber, int count, string search, CancellationToken cancellationToken)
        {

            var query = Repository.TableNoTracking;

            if (!string.IsNullOrEmpty(search))
            {
                var entityType = typeof(TEntity);
                var stringProperties = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string)) // Filter only string properties
                    .ToList();

                if (stringProperties.Any())
                {
                    // Dynamically build the query condition
                    var predicate = PredicateBuilder.New<TEntity>(x => false); // Start with an always-false predicate

                    foreach (var property in stringProperties)
                    {
                        var parameter = Expression.Parameter(entityType, "x");
                        var propertyAccess = Expression.Property(parameter, property);
                        var searchExpression = Expression.Call(propertyAccess, "Contains", null, Expression.Constant(search, typeof(string)));
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(searchExpression, parameter);

                        predicate = predicate.Or(lambda); // Combine each condition using OR
                    }

                    query = query.Where(predicate);
                }
            }

            var list = await query
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            if (list == null)
                return NotFound();

            var totalCount = await query.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .CountAsync(cancellationToken);
            return Ok(new GlobalGridResult<TSelectDto> { Data = list, TotalCount = totalCount });
        }

        [HttpGet("{id}")]
        public virtual async Task<ApiResult<TSelectDto>> Get(TKey id, CancellationToken cancellationToken)
        {
            var dto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }

        [HttpPost]
        public virtual async Task<ApiResult<TSelectDto>> Create(TDto dto, CancellationToken cancellationToken)
        {
            var model = dto.ToEntity(Mapper);

            await Repository.AddAsync(model, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        [HttpPut]
        public virtual async Task<ApiResult<TSelectDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
        {
            var model = await Repository.GetByIdAsync(cancellationToken, id);

            model = dto.ToEntity(Mapper, model);

            await Repository.UpdateAsync(model, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id}")]
        public virtual async Task<ApiResult> Delete(TKey id, CancellationToken cancellationToken)
        {
            var model = await Repository.GetByIdAsync(cancellationToken, id);

            await Repository.SoftDeleteAsync(model, cancellationToken);

            return Ok();
        }
    }

    public class CrudController<TDto, TSelectDto, TEntity> : CrudController<TDto, TSelectDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TSelectDto : BaseDto<TSelectDto, TEntity, int>, new()
        where TEntity : class, IEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class CrudController<TDto, TEntity> : CrudController<TDto, TDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TEntity : class, IEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
