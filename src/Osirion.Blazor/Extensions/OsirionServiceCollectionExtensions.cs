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
            var contentSection = configuration.GetSection("Osirion:Cms");
            if (contentSection.Exists())
            {
                builder.UseContent(configuration);
            }

            // CMS Admin configuration
            var cmsAdminSection = configuration.GetSection("Osirion:Cms:GitHub:Admin");
            if (cmsAdminSection.Exists())
            {
                builder.UseCmsAdmin(configuration);
            }

            // Analytics configuration
            var analyticsSection = configuration.GetSection("Osirion:Analytics");
            if (analyticsSection.Exists())
            {
                builder.UseAnalytics(configuration);
            }

            // Navigation configuration
            var navigationSection = configuration.GetSection("Osirion:Navigation");
            if (navigationSection.Exists())
            {
                builder.UseNavigation(configuration);
            }

            // Theming configuration
            var themingSection = configuration.GetSection("Osirion:Theming");
            if (themingSection.Exists())
            {
                builder.UseTheming(configuration);
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