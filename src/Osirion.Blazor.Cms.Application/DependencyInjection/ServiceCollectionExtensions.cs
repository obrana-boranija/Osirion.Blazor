using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Application.Queries.Directory;
using Osirion.Blazor.Cms.Application.Commands.Directory;
using Osirion.Blazor.Cms.Domain.Entities;

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

        // The implementations will need to be created
        //services.AddScoped<IQueryHandler<GetContentByUrlQuery, ContentItem?>, GetContentByUrlQueryHandler>();
        //services.AddScoped<IQueryHandler<GetAllContentQuery, IReadOnlyList<ContentItem>>, GetAllContentQueryHandler>();
        //services.AddScoped<IQueryHandler<GetContentTagsQuery, IReadOnlyList<Domain.Repositories.ContentTag>>, GetContentTagsQueryHandler>();
        //services.AddScoped<IQueryHandler<GetContentCategoriesQuery, IReadOnlyList<Domain.Repositories.ContentCategory>>, GetContentCategoriesQueryHandler>();

        // Directory queries(added)
        //services.AddScoped<IQueryHandler<GetDirectoryByIdQuery, DirectoryItem?>, GetDirectoryByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetDirectoryByPathQuery, DirectoryItem?>, GetDirectoryByPathQueryHandler>();
        services.AddScoped<IQueryHandler<GetDirectoryTreeQuery, IReadOnlyList<DirectoryItem>>, GetDirectoryTreeQueryHandler>();
    }


}