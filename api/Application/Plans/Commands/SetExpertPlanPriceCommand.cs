using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands;

public record SetExpertPlanPriceCommand(long ExpertId, long PlanId, decimal Price) : ICommand<ServiceResult>;

public class SetExpertPlanPriceCommandHandler(IRepository<ExpertPlanPrice> priceRepository) : ICommandHandler<SetExpertPlanPriceCommand, ServiceResult>
{
    private readonly IRepository<ExpertPlanPrice> _priceRepository = priceRepository;

    public async Task<ServiceResult> Handle(SetExpertPlanPriceCommand request, CancellationToken cancellationToken)
    {
        var existingPrice = await _priceRepository.Table
            .FirstOrDefaultAsync(x => x.ExpertId == request.ExpertId &&
                                      x.PlanId == request.PlanId &&
                                      x.IsActive, cancellationToken);

        if (existingPrice != null)
        {
            existingPrice.UpdatePrice(request.Price);
            await _priceRepository.UpdateAsync(existingPrice, cancellationToken);
            return ServiceResult.Ok("قیمت با موفقیت بروزرسانی شد");
        }

        var newPrice = ExpertPlanPrice.Create(request.ExpertId, request.PlanId, request.Price);
        await _priceRepository.AddAsync(newPrice, cancellationToken);

        return ServiceResult.Ok("قیمت جدید با موفقیت ثبت شد");
    }
}
