using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events;
using Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering domain event components
/// </summary>
public static class DomainEventRegistrationExtensions
{
    /// <summary>
    /// Adds domain event services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        // Register the domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register all event handlers
        RegisterEventHandlers(services);

        return services;
    }

    private static void RegisterEventHandlers(IServiceCollection services)
    {
        // Content event handlers
        services.AddScoped<IDomainEventHandler<ContentCreatedEvent>, ContentCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ContentUpdatedEvent>, ContentUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ContentDeletedEvent>, ContentDeletedEventHandler>();
        services.AddScoped<IDomainEventHandler<ContentStatusChangedEvent>, ContentStatusChangedEventHandler>();

        // Directory event handlers
        services.AddScoped<IDomainEventHandler<DirectoryCreatedEvent>, DirectoryCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<DirectoryUpdatedEvent>, DirectoryUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<DirectoryDeletedEvent>, DirectoryDeletedEventHandler>();
    }
}