using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Osirion.Blazor.Cms.Infrastructure.Events;

/// <summary>
/// Implementation of the domain event dispatcher
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    /// <summary>Performs the DomainEventDispatcher operation.</summary>
    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a domain event to the handlers registered for its runtime type, so a handler registered as
    /// <c>IDomainEventHandler&lt;ContentCreatedEvent&gt;</c> runs even when the event is passed as an <see cref="IDomainEvent"/>.
    /// </summary>
    public async Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent
    {
        if (domainEvent is null)
            throw new ArgumentNullException(nameof(domainEvent));

        var eventType = domainEvent.GetType();
        _logger.LogDebug("Dispatching domain event {EventType}", eventType.Name);

        try
        {
            var invoker = HandlerInvokers.GetOrAdd(eventType, CreateInvoker);
            var handlers = _serviceProvider.GetServices(invoker.HandlerType).Where(handler => handler is not null).ToList();

            if (handlers.Count == 0)
            {
                _logger.LogWarning("No handlers found for domain event {EventType}", eventType.Name);
                return;
            }

            // Execute each handler
            foreach (var handler in handlers)
            {
                try
                {
                    await invoker.Invoke(handler!, domainEvent);
                }
                catch (Exception ex)
                {
                    // Log exception but continue processing other handlers
                    _logger.LogError(ex, "Error handling domain event {EventType} in handler {HandlerType}",
                        eventType.Name, handler!.GetType().Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event {EventType}", eventType.Name);
            throw;
        }
    }

    private static readonly ConcurrentDictionary<Type, HandlerInvoker> HandlerInvokers = new();

    private static HandlerInvoker CreateInvoker(Type eventType)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handlerParameter = Expression.Parameter(typeof(object), "handler");
        var eventParameter = Expression.Parameter(typeof(object), "domainEvent");
        var call = Expression.Call(
            Expression.Convert(handlerParameter, handlerType),
            handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!,
            Expression.Convert(eventParameter, eventType));
        var invoke = Expression.Lambda<Func<object, object, Task>>(call, handlerParameter, eventParameter).Compile();
        return new HandlerInvoker(handlerType, invoke);
    }

    private sealed record HandlerInvoker(Type HandlerType, Func<object, object, Task> Invoke);
}
