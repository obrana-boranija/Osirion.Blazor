using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        AddValidatorAdapters(services, typeof(IValidator<object>).Assembly);

        return services;
    }

    /// <summary>
    /// Registers an <see cref="IValidator{T}"/> adapter for each type that has a FluentValidation validator.
    /// </summary>
    /// <remarks>
    /// An open-generic adapter would also match commands that have no validator, and resolving it would then fail
    /// because the adapter's FluentValidation validator cannot be created, so those commands could not be
    /// dispatched at all. The dispatcher treats a missing adapter as "no validation".
    /// </remarks>
    private static void AddValidatorAdapters(IServiceCollection services, Assembly assembly)
    {
        var validatedTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsGenericTypeDefinition: false })
            .SelectMany(type => type.GetInterfaces())
            .Where(contract => contract.IsGenericType
                && contract.GetGenericTypeDefinition() == typeof(FluentValidation.IValidator<>))
            .Select(contract => contract.GetGenericArguments()[0])
            .Distinct();

        foreach (var validatedType in validatedTypes)
        {
            services.TryAddTransient(
                typeof(IValidator<>).MakeGenericType(validatedType),
                typeof(FluentValidationAdapter<>).MakeGenericType(validatedType));
        }
    }
}
