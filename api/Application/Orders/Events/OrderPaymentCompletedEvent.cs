using Application.Cqrs.Commands;
using Application.Plans.Commands;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Orders;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Events
{
    public class OrderPaymentCompletedEvent(long userId, ICollection<OrderItem> orderItems) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public ICollection<OrderItem> OrderItems { get; } = orderItems;
    }

    public class VerifyAndAssignOrderEventHandler
    : IDomainEventAction<OrderPaymentCompletedEvent>
{
    private readonly IRepository<UserPlan> _userPlanRepository;
    private readonly IRepository<ExpertPlanPrice> _expertPlanPriceRepository;
    private readonly IRepository<Plan> _planRepository;
    private readonly ICommandDispatcher _commandDispatcher;

    public VerifyAndAssignOrderEventHandler(
        IRepository<UserPlan> userPlanRepository,
        IRepository<ExpertPlanPrice> expertPlanPriceRepository,
        IRepository<Plan> planRepository,
        ICommandDispatcher commandDispatcher)
    {
        _userPlanRepository = userPlanRepository;
        _expertPlanPriceRepository = expertPlanPriceRepository;
        _planRepository = planRepository;
        _commandDispatcher = commandDispatcher;
    }

    public async Task ExecuteAsync(OrderPaymentCompletedEvent @event, CancellationToken cancellationToken)
    {
        var userId = @event.UserId;

        // Verify payment status (assuming it's done, so we continue with the assignment)
        var userPlans = await _userPlanRepository.Table.Where(t => t.UserId == userId).ToListAsync();

        // First, assign plans if the user has ordered them
        foreach (var orderItem in @event.OrderItems)
        {
            if (orderItem.ItemType == OrderItemType.Plan)
            {
                var planInDb = await _planRepository.Table
                    .Where(t => t.Id == orderItem.ObjectId)
                    .FirstOrDefaultAsync(cancellationToken);

                // Assuming we have a command to assign plans to users
                await _commandDispatcher.SendAsync(new AssginPlanToUserCommand(planInDb.Id, userId));
            }
        }

        // Second, assign experts if the user has ordered expert plans
        foreach (var orderItem in @event.OrderItems)
        {
            if (orderItem.ItemType == OrderItemType.ExpertPlanPrice)
            {
                var expertPlanInDb = await _expertPlanPriceRepository.Table
                    .Include(t => t.Expert)
                    .Where(t => t.Id == orderItem.ObjectId)
                    .FirstOrDefaultAsync(cancellationToken);

                var userPlanId = userPlans
                    .Where(a => a.PlanId == expertPlanInDb.PlanId)
                    .Select(t => t.Id)
                    .FirstOrDefault();

                await _commandDispatcher.SendAsync(new CreateUserPlanExpertCommand(
                    new Plans.DTOs.CreateUserPlanExpertCommandDTO
                    {
                        ExpertId = expertPlanInDb.ExpertId,
                        UserPlanId = userPlanId,
                        Price = expertPlanInDb.Price,
                        Specialization = expertPlanInDb.Expert.JobTitle,
                    }));
            }
        }
    }
}

}
