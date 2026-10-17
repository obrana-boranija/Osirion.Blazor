using BlazorJSComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Web.Options;

namespace Osirion.Blazor.Cms.Web.DependencyInjection;

/// <summary>
/// Extension methods for adding Osirion CMS content services
/// </summary>
public static class OsirionContentServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS content services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure content services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        Action<IContentBuilder> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        var (configuration, logger) = ResolveConfiguration(services);

        // Add core CMS services
        services.AddCms(configuration);
        AddScriptComponents(services);

        var builder = new ContentBuilder(services, configuration, logger);

        configure(builder);

        return services;
    }

    private static (IConfiguration Configuration, ILogger<ContentBuilder> Logger) ResolveConfiguration(IServiceCollection services)
    {
        var descriptor = services.LastOrDefault(item => item.ServiceType == typeof(IConfiguration) && !item.IsKeyedService);
        if (descriptor is null)
        {
            throw new InvalidOperationException(
                "AddOsirionContent(Action<IContentBuilder>) needs an IConfiguration registration and found none. " +
                "Use AddOsirionContent(IConfiguration) to pass the configuration explicitly.");
        }

        // An instance is read directly, so no second set of singletons is created.
        if (descriptor.ImplementationInstance is IConfiguration instance)
        {
            return (instance, NullLogger<ContentBuilder>.Instance);
        }

        // The hosts register IConfiguration through a factory, which needs a provider to run. One is built once and
        // left undisposed: disposing it would dispose the host's configuration, which the factory returns.
        var provider = services.BuildServiceProvider();
        return (provider.GetRequiredService<IConfiguration>(), provider.GetRequiredService<ILogger<ContentBuilder>>());
    }

    /// <summary>
    /// Adds Osirion CMS content services to the service collection using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Add core CMS services
        services.AddCms(configuration);
        AddScriptComponents(services);

        // Configure providers based on configuration
        var cmsSection = configuration.GetSection("Osirion:Cms");

        // Configure GitHub provider if present
        var githubSection = cmsSection.GetSection("Web:GitHub");
        if (githubSection.Exists())
        {
            //services.AddGitHubContentProvider(configuration);
            services.AddGitHubProvidersFromConfiguration(configuration);
        }

        // Configure FileSystem provider if present
        if (configuration.GetSection(FileSystemOptions.WebSection).Exists()
            || configuration.GetSection(FileSystemOptions.Section).Exists())
        {
            services.AddFileSystemContentProvider(configuration);
        }

        return services;
    }

    /// <summary>
    /// Configures SEO metadata options for the Osirion CMS
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure SEO metadata options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionSeoMetadata(
        this IServiceCollection services,
        Action<SeoMetadataOptions> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);
        return services;
    }

    /// <summary>
    /// Configures SEO metadata options for the Osirion CMS using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration section containing SEO metadata options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionSeoMetadata(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<SeoMetadataOptions>(configuration);
        return services;
    }

    // The table of contents loads its script as a JS component, which needs the element reference scope. A host
    // that composes the content services without AddOsirion would otherwise fail on the first article.
    private static void AddScriptComponents(IServiceCollection services)
    {
        if (!services.Any(descriptor => descriptor.ServiceType == typeof(JSElementReferenceScope)))
        {
            services.AddJSComponents();
        }
    }
}
