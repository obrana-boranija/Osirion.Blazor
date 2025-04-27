using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Internal;
using Osirion.Blazor.Theming;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor services
/// </summary>
public static class OsirionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirion(
        this IServiceCollection services,
        Action<IOsirionBuilder>? configure = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Create the builder
        var builder = new OsirionBuilder(services);

        // Apply defaults
        ApplyDefaults(builder);

        // Apply custom configuration
        configure?.Invoke(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor services from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirion(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return services.AddOsirion(builder =>
        {
            // Content configuration
            var contentSection = configuration.GetSection("Osirion:Content");
            if (contentSection.Exists())
            {
                builder.UseContent(content =>
                {
                    var githubSection = contentSection.GetSection("GitHub");
                    if (githubSection.Exists())
                    {
                        content.AddGitHub(options =>
                        {
                            githubSection.Bind(options);
                        });
                    }

                    var fileSystemSection = contentSection.GetSection("FileSystem");
                    if (fileSystemSection.Exists())
                    {
                        content.AddFileSystem(options =>
                        {
                            fileSystemSection.Bind(options);
                        });
                    }
                });
            }

            // Analytics configuration
            var analyticsSection = configuration.GetSection("Osirion:Analytics");
            if (analyticsSection.Exists())
            {
                builder.UseAnalytics(analytics =>
                {
                    var claritySection = analyticsSection.GetSection("Clarity");
                    if (claritySection.Exists())
                    {
                        analytics.AddClarity(options =>
                        {
                            claritySection.Bind(options);
                        });
                    }

                    var matomoSection = analyticsSection.GetSection("Matomo");
                    if (matomoSection.Exists())
                    {
                        analytics.AddMatomo(options =>
                        {
                            matomoSection.Bind(options);
                        });
                    }
                });
            }

            // Navigation configuration
            var navigationSection = configuration.GetSection("Osirion:Navigation");
            if (navigationSection.Exists())
            {
                builder.UseNavigation(navigation =>
                {
                    var enhancedSection = navigationSection.GetSection("Enhanced");
                    if (enhancedSection.Exists())
                    {
                        navigation.UseEnhancedNavigation(options =>
                        {
                            enhancedSection.Bind(options);
                        });
                    }

                    var scrollSection = navigationSection.GetSection("ScrollToTop");
                    if (scrollSection.Exists())
                    {
                        navigation.AddScrollToTop(options =>
                        {
                            scrollSection.Bind(options);
                        });
                    }
                });
            }

            // Theming configuration
            var themingSection = configuration.GetSection("Osirion:Theming");
            if (themingSection.Exists())
            {
                builder.UseTheming(theming =>
                {
                    theming.Configure(options =>
                    {
                        themingSection.Bind(options);
                    });
                });
            }
        });
    }

    private static void ApplyDefaults(IOsirionBuilder builder)
    {
        // Add basic navigation with enhanced features
        builder.UseNavigation(navigation =>
        {
            navigation.UseEnhancedNavigation();
        });

        // Use default theming (no framework integration)
        builder.UseTheming(theming =>
        {
            theming.UseFramework(CssFramework.None);
        });
    }
}