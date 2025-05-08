using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Application.Commands;
using Osirion.Blazor.Cms.Admin.Builders;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Queries;

namespace Osirion.Blazor.Cms.Admin.DependencyInjection;

/// <summary>
/// Extension methods for adding Osirion CMS Admin services to the service collection
/// </summary>
public static class OsirionCmsAdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure the admin builder</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<IOsirionCmsAdminBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Add core services from Domain and Application layers
        services.AddCoreServices();

        // Create builder and apply configuration
        var serviceProvider = services.BuildServiceProvider();
        var builder = new OsirionCmsAdminBuilder(
            services,
            serviceProvider.GetRequiredService<IConfiguration>(),
            serviceProvider.GetRequiredService<ILogger<OsirionCmsAdminBuilder>>());

        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection with the configuration from appsettings
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Add core services from Domain and Application layers
        services.AddCoreServices();

        // Configure from appsettings
        services.Configure<CmsAdminOptions>(configuration.GetSection("Osirion:Cms:Admin"));

        // Create builder with configuration-based setup
        var serviceProvider = services.BuildServiceProvider();
        var builder = new OsirionCmsAdminBuilder(
            services,
            configuration,
            serviceProvider.GetRequiredService<ILogger<OsirionCmsAdminBuilder>>());

        // Configure providers based on configuration
        var cmsSection = configuration.GetSection("Osirion:Cms:Admin");

        // Configure GitHub provider if present
        var githubSection = cmsSection.GetSection("GitHub");
        if (githubSection.Exists())
        {
            builder.UseGitHubProvider();
        }

        // Configure FileSystem provider if present
        var fileSystemSection = cmsSection.GetSection("FileSystem");
        if (fileSystemSection.Exists())
        {
            builder.UseFileSystemProvider();
        }

        return services;
    }

    /// <summary>
    /// Adds core services required by the CMS Admin
    /// </summary>
    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register command and query dispatchers from Application layer
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Register command handlers
        services.AddApplicationCommandHandlers();

        // Register query handlers
        services.AddApplicationQueryHandlers();

        return services;
    }

    /// <summary>
    /// Registers application command handlers
    /// </summary>
    private static IServiceCollection AddApplicationCommandHandlers(this IServiceCollection services)
    {
        // Register command handlers from the Application layer
        // Scan assembly for command handlers
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;

        var handlerTypes = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                        || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                        || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, handlerType);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers application query handlers
    /// </summary>
    private static IServiceCollection AddApplicationQueryHandlers(this IServiceCollection services)
    {
        // Register query handlers from the Application layer
        // Scan assembly for query handlers
        var applicationAssembly = typeof(IQueryHandler<,>).Assembly;

        var handlerTypes = applicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, handlerType);
            }
        }

        return services;
    }
}