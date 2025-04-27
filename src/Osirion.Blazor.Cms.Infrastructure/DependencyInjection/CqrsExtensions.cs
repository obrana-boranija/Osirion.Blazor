using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Validation;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering CQRS components
/// </summary>
public static class CqrsExtensions
{
    /// <summary>
    /// Adds CQRS components to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCqrs(this IServiceCollection services)
    {
        // Register dispatchers
        services.AddScoped<ICommandDispatcher, ValidatingCommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register command handlers
        services.Scan(scan => scan
            .FromAssemblyOf<ICommandHandler<ICommand>>()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register query handlers
        services.Scan(scan => scan
            .FromAssemblyOf<IQueryHandler<IQuery<object>, object>>()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register validators
        services.Scan(scan => scan
            .FromAssemblyOf<IValidator<object>>()
            .AddClasses(classes => classes.AssignableTo(typeof(FluentValidation.IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // Register validator adapters
        services.AddTransient(typeof(IValidator<>), typeof(FluentValidationAdapter<>));

        return services;
    }
}