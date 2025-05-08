using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Application.Commands;
using Osirion.Blazor.Cms.Admin.Builders;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Queries.Content;

namespace Osirion.Blazor.Cms.Admin.DependencyInjection;

/// <summary>
/// Extension methods for configuring Osirion CMS Admin services
/// </summary>
public static class OsirionCmsAdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection with builder pattern configuration
    /// </summary>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<IAdminBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Add core services from Domain and Application
        services.AddCoreServices();

        // Create builder and apply configuration
        var serviceProvider = services.BuildServiceProvider();
        var builder = new AdminBuilder(
            services,
            serviceProvider.GetRequiredService<IConfiguration>(),
            serviceProvider.GetRequiredService<ILogger<AdminBuilder>>());

        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection with configuration-based setup
    /// </summary>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<GitHubAdminOptions>? configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Register configuration options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<GitHubAdminOptions>(
                configuration.GetSection("Osirion:Cms:GitHub:Admin"));
        }

        // Add core services
        services.AddCoreServices();

        // Register core infrastructure
        services.AddSingleton<IEventPublisher, EventBus>();
        services.AddSingleton<IEventSubscriber>(provider =>
            provider.GetRequiredService<IEventPublisher>() as EventBus);
        services.AddScoped<CmsState>();
        services.AddScoped<StatePersistenceService>();

        // Register adapters with factory pattern
        services.AddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        services.AddScoped<IContentRepositoryAdapter>(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register feature services
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ContentBrowserService>();
        services.AddScoped<IContentEditorService, ContentEditorService>();
        services.AddScoped<RepositoryService>();

        // Register view models (omitted for brevity)

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register domain and application services
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register command handlers
        services.AddScoped<ICommandHandler<SaveContentCommand, SaveContentResult>, SaveContentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteContentCommand>, DeleteContentCommandHandler>();
        services.AddScoped<ICommandHandler<CreateContentCommand, CreateContentResult>, CreateContentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateContentCommand, UpdateContentResult>, UpdateContentCommandHandler>();

        // Register query handlers
        services.AddScoped<IQueryHandler<GetContentByIdQuery, ContentItemResult>, GetContentByIdQueryHandler>();
        services.AddScoped<IQueryHandler<SearchContentQuery, SearchContentResult>, SearchContentQueryHandler>();
        services.AddScoped<IQueryHandler<GetContentByPathQuery, ContentItemResult>, GetContentByPathQueryHandler>();

        return services;
    }
}