using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Application.Validation.Content;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOsirionCqrs(this IServiceCollection services)
    {
        // Register dispatchers
        services.AddScoped<ICommandDispatcher, ValidatingCommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register command handlers using Scrutor
        services.Scan(scan => scan
            .FromAssemblyOf<CreateContentCommandHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register query handlers using Scrutor
        services.Scan(scan => scan
            .FromAssemblyOf<GetContentByIdQueryHandler>()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register validators using Scrutor
        services.Scan(scan => scan
            .FromAssemblyOf<CreateContentCommandValidator>()
            .AddClasses(classes => classes.AssignableTo(typeof(FluentValidation.IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // Register validator adapters
        services.AddTransient(typeof(IValidator<>), typeof(FluentValidationAdapter<>));

        return services;
    }
}