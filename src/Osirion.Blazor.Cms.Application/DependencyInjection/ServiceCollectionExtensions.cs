using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Commands.Directory;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Application.Queries.Directory;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOsirionCqrs(this IServiceCollection services)
    {
        // Register dispatchers
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register validating command dispatcher
        services.AddScoped<ICommandDispatcher, ValidatingCommandDispatcher>();

        // Register command handlers
        RegisterCommandHandlers(services);

        // Register query handlers
        RegisterQueryHandlers(services);

        // Register FluentValidation adapters
        services.AddScoped(typeof(IValidator<>), typeof(FluentValidationAdapter<>));

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        // Content commands
        services.AddScoped<ICommandHandler<CreateContentCommand>, CreateContentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateContentCommand>, UpdateContentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteContentCommand>, DeleteContentCommandHandler>();

        // Directory commands
        services.AddScoped<ICommandHandler<CreateDirectoryCommand>, CreateDirectoryCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateDirectoryCommand>, UpdateDirectoryCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteDirectoryCommand>, DeleteDirectoryCommandHandler>();
        services.AddScoped<ICommandHandler<MoveDirectoryCommand>, MoveDirectoryCommandHandler>();
    }

    private static void RegisterQueryHandlers(IServiceCollection services)
    {
        // Content queries
        services.AddScoped<IQueryHandler<GetContentByIdQuery, ContentItem?>, GetContentByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetContentByPathQuery, ContentItem?>, GetContentByPathQueryHandler>();
        services.AddScoped<IQueryHandler<SearchContentQuery, IReadOnlyList<ContentItem>>, SearchContentQueryHandler>();

        // Directory queries
        services.AddScoped<IQueryHandler<GetDirectoryByPathQuery, DirectoryItem?>, GetDirectoryByPathQueryHandler>();
        services.AddScoped<IQueryHandler<GetDirectoryTreeQuery, IReadOnlyList<DirectoryItem>>, GetDirectoryTreeQueryHandler>();
    }
}