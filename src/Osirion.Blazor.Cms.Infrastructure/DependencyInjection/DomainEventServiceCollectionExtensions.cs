using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events;
using Osirion.Blazor.Cms.Infrastructure.Events.Handlers;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

public static class DomainEventServiceCollectionExtensions
{
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