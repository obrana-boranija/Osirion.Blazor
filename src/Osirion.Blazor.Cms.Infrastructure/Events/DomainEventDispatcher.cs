using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Infrastructure.Events;

/// <summary>
/// Implementation of the domain event dispatcher
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent
    {
        if (domainEvent is null)
            throw new ArgumentNullException(nameof(domainEvent));

        _logger.LogDebug("Dispatching domain event {EventType}", typeof(T).Name);

        try
        {
            // Resolve all handlers for this event type
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<T>>();

            if (!handlers.Any())
            {
                _logger.LogWarning("No handlers found for domain event {EventType}", typeof(T).Name);
                return;
            }

            // Execute each handler
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(domainEvent);
                }
                catch (Exception ex)
                {
                    // Log exception but continue processing other handlers
                    _logger.LogError(ex, "Error handling domain event {EventType} in handler {HandlerType}",
                        typeof(T).Name, handler.GetType().Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event {EventType}", typeof(T).Name);
            throw;
        }
    }
}