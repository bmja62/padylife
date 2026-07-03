using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Application.Cqrs.Events.Dispatcher.Application.Cqrs.Events.Dispatcher;
using Common.Utilities;
using Data;
using Data.Repositories.Extentions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Application.Cqrs.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        //private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public TransactionBehavior(ApplicationDbContext dbContext,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger,
            IDomainEventDispatcher domainEventDispatcher)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(ApplicationDbContext));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
            _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentException(nameof(DomainEventDispatcher));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    await using var transaction = await _dbContext.BeginTransactionAsync();
                    using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                    {
                        _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);


                        // Dispatch Domain Events قبل از Commit
                        var domainEvents = _dbContext.GetDomainEvents();
                        if (domainEvents.Any())
                        {
                            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
                            _dbContext.ClearDomainEvents(); // پاک کردن DomainEvent ها بعد از Dispatch
                        }

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
