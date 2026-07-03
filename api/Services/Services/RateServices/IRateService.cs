using Common;
using Data.Contracts;
using Entities.Common;
using Entities.Rates;
using Microsoft.EntityFrameworkCore;
using Services.Services.CommentServices.DTOs;
using Services.Services.RateServices.DTOs;

namespace Services.Services.RateServices
{
    public interface IRateService
    {
        Task<Rate> AddRateAsync(long userId, CreateRateDTO dto);
        Task<Rate> UpdateRateAsync(long userId, long entityId, EntityType entityType, UpdateRateDTO dto);
        Task<double> GetAverageRatingAsync(long entityId, EntityType entityType);
        Task<bool> IsRateExist(long userId, long entityId, EntityType entityType);
    }
    public class RateService : IRateService, IScopedDependency
    {
        private readonly IRepository<Rate> _rateRepository;

        public RateService(IRepository<Rate> rateRepository)
        {
            _rateRepository = rateRepository;
        }

        public async Task<Rate> AddRateAsync(long userId, CreateRateDTO dto)
        {
            var exists = await _rateRepository.Table.AnyAsync(r =>
                r.CreatedByUserId == userId &&
                r.EntityId == dto.EntityId &&
                r.EntityType == dto.EntityType);

            if (exists)
                return null;

            var rate = new Rate(dto.EntityId, dto.EntityType, dto.RatingValue, userId);
            await _rateRepository.AddAsync(rate, CancellationToken.None);
            return rate;
        }

        public async Task<bool> IsRateExist(long userId, long entityId, EntityType entityType)
        {
            var exists = await _rateRepository.Table.AnyAsync(r =>
                r.CreatedByUserId == userId &&
                r.EntityId == entityId &&
                r.EntityType == entityType);

            if (exists)
                return true;

            return false;
        }


        public async Task<double> GetAverageRatingAsync(long entityId, EntityType entityType)
        {
            var rates = await _rateRepository.Table
                .Where(r => r.EntityId == entityId && r.EntityType == entityType)
                .ToListAsync();

            return Rate.CalculateAverage(rates);
        }

        public async Task<Rate> UpdateRateAsync(long userId, long entityId, EntityType entityType, UpdateRateDTO dto)
        {
            var rate = await _rateRepository.Table
                .FirstOrDefaultAsync(r => r.CreatedByUserId == userId && r.EntityId == entityId && r.EntityType == entityType);

            if (rate == null)
                return null;

            rate.RatingValue = dto.RatingValue;
            await _rateRepository.UpdateAsync(rate, CancellationToken.None);

            return rate;
        }
    }

}
