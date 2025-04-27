using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering domain events
/// </summary>
public static class DomainEventExtensions
{
    /// <summary>
    /// Adds domain event services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionDomainEvents(this IServiceCollection services)
    {
        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register event handlers
        services.Scan(scan => scan
            .FromAssemblyOf<IDomainEventHandler<IDomainEvent>>()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}