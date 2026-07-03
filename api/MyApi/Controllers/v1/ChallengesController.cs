using Asp.Versioning;
using AutoMapper;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Challange;
using Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadyLife.Api.Models.DTOs;
using Services;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر چالش ها
    /// </summary>
    /// <param name="challengeRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class ChallengesController(IHttpContextAccessor accessor, IRepository<User> userRepository, IRepository<Challange> challengeRepository, IMapper mapper) : CrudController<ChallangeDTO, ChallangeDTO, Challange, long>(challengeRepository, mapper)
    {

        public override async Task<ApiResult<ChallangeDTO>> Get(long id, CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            var challenge = await base.Get(id, cancellationToken);
            bool hasParticipantByMe = challengeRepository.Table.Include(t => t.Logs).Where(t => t.Id == challenge.Data.Id).SelectMany(a => a.Logs).Any(t => t.UserId == currentUserId);
            challenge.Data.HasParticipantByMe = hasParticipantByMe;
            return challenge;
        }
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.Specialist}")]
        public override Task<ApiResult<ChallangeDTO>> Create(ChallangeDTO dto, CancellationToken cancellationToken)
        {
            return base.Create(dto, cancellationToken);
        }
        /// <summary>
        /// دریافت چالش ها با فیلتر
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <param name="search"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<ChallangeDTO>>> GetAllByFilter(int pageNumber, int count, string search, ChallengeType? type, bool? allUsers , CancellationToken cancellationToken)
        {
            var query = challengeRepository.Table.Include(t => t.Logs).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Title.Contains(search));
            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            
            if (allUsers.HasValue && !allUsers.Value)
                query = query.Where(t => t.CreatedByUserId == currentUserId);    
            
            var data = await query
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .Select(t => new ChallangeDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ImageUrl = t.ImageUrl,
                    Type = t.Type,
                    ParticipantCount = t.ParticipantCount,
                    HasParticipantByMe = t.Logs.Any(t => t.UserId == currentUserId)
                }).ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync(cancellationToken);
            return (ServiceResult.Ok(new GlobalGridResult<ChallangeDTO>
            {
                Data = data,
                TotalCount = totalCount
            })).ToApiResult();
        }

        /// <summary>
        /// پیوستن به چالش
        /// </summary>
        /// <param name="challengId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> JoinChallenge([FromQuery] long challengId, CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            var challengeInDb = await challengeRepository.Table.Include(t => t.Logs).Where(t => t.Id == challengId).FirstOrDefaultAsync();
            if (challengeInDb != null && challengeInDb.Logs.Any(t => t.UserId == currentUserId))
            {
                return ServiceResult.BadRequest("شما در این چالش شرکت کرده اید").ToApiResult();
            }

            challengeInDb.ActivityLog(currentUserId);
            await challengeRepository.UpdateAsync(challengeInDb, cancellationToken);
            return ServiceResult.Ok().ToApiResult();
        }

        /// <summary>
        /// چالش های من
        /// </summary>
        /// <param name="globalGrid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public virtual async Task<ApiResult> GetChallengesByToken([FromQuery] GlobalGrid globalGrid, CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            
            var challengIds = await userRepository.Table.Include(t => t.ChallangeLogs).Where(t => t.Id == currentUserId).SelectMany(t => t.ChallangeLogs).Select(t => t.ChallengId).ToListAsync();

            var query = challengeRepository.Table.Where(t => challengIds.Contains(t.Id));

            if (!string.IsNullOrEmpty(globalGrid.Search))
                query = query.Where(t => t.Title.Contains(globalGrid.Search));

            var challenges = await query.Select(t => new ChallangeDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                Type = t.Type,
                ParticipantCount = t.ParticipantCount,
                HasParticipantByMe = t.Logs.Any(t => t.UserId == currentUserId)
            })
                .Skip((globalGrid.PageNumber.Value - 1) * globalGrid.Count.Value)
                .Take(globalGrid.Count.Value)
                .ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<ChallangeDTO>
            {
                Data = challenges,
                TotalCount = totalCount
            }).ToApiResult();
        }



    }
}
