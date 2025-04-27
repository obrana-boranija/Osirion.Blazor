using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Application.Validation;

namespace Osirion.Blazor.Cms.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering CQRS services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CQRS components and handlers to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCqrs(this IServiceCollection services)
    {
        // Register dispatchers
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register validating command dispatcher as primary implementation
        services.AddScoped<ICommandDispatcher, ValidatingCommandDispatcher>();

        // Register command handlers
        RegisterCommandHandlers(services);

        // Register query handlers
        RegisterQueryHandlers(services);

        // Register FluentValidation adapters
        services.AddScoped(typeof(Validation.IValidator<>), typeof(FluentValidationAdapter<>));

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        // Content commands
        services.AddScoped<ICommandHandler<CreateContentCommand>, CreateContentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateContentCommand>, UpdateContentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteContentCommand>, DeleteContentCommandHandler>();

        // Directory commands
        // The implementations will need to be created
        // services.AddScoped<ICommandHandler<CreateDirectoryCommand>, CreateDirectoryCommandHandler>();
        // services.AddScoped<ICommandHandler<UpdateDirectoryCommand>, UpdateDirectoryCommandHandler>();
        // services.AddScoped<ICommandHandler<DeleteDirectoryCommand>, DeleteDirectoryCommandHandler>();
        // services.AddScoped<ICommandHandler<MoveDirectoryCommand>, MoveDirectoryCommandHandler>();
    }

    private static void RegisterQueryHandlers(IServiceCollection services)
    {
        // Content queries
        services.AddScoped<IQueryHandler<GetContentByIdQuery, Domain.Entities.ContentItem?>, GetContentByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetContentByPathQuery, Domain.Entities.ContentItem?>, GetContentByPathQueryHandler>();
        services.AddScoped<IQueryHandler<SearchContentQuery, IReadOnlyList<Domain.Entities.ContentItem>>, SearchContentQueryHandler>();

        // The implementations will need to be created
        //services.AddScoped<IQueryHandler<GetContentByUrlQuery, Domain.Entities.ContentItem?>, GetContentByUrlQueryHandler>();
        //services.AddScoped<IQueryHandler<GetAllContentQuery, IReadOnlyList<Domain.Entities.ContentItem>>, GetAllContentQueryHandler>();
        //services.AddScoped<IQueryHandler<GetContentTagsQuery, IReadOnlyList<Domain.Repositories.ContentTag>>, GetContentTagsQueryHandler>();
        //services.AddScoped<IQueryHandler<GetContentCategoriesQuery, IReadOnlyList<Domain.Repositories.ContentCategory>>, GetContentCategoriesQueryHandler>();

        // The implementations will need to be created
        // services.AddScoped<IQueryHandler<GetDirectoryByIdQuery, Domain.Entities.DirectoryItem?>, GetDirectoryByIdQueryHandler>();
        // services.AddScoped<IQueryHandler<GetDirectoryByPathQuery, Domain.Entities.DirectoryItem?>, GetDirectoryByPathQueryHandler>();
        // services.AddScoped<IQueryHandler<GetDirectoryTreeQuery, IReadOnlyList<Domain.Entities.DirectoryItem>>, GetDirectoryTreeQueryHandler>();
    }
}