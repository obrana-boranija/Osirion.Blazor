using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Theming.Internal;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Extensions;

/// <summary>
/// Extension methods for configuring theming services
/// </summary>
public static class ThemingServiceCollectionExtensions
{
    /// <summary>
    /// Adds theming services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure theming services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionTheming(
        this IServiceCollection services,
        Action<IThemingBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Create builder and apply configuration
        var builder = new ThemingBuilder(services);
        configure(builder);

        // Register theme service
        services.AddSingleton<IThemeService, ThemeService>();

        return services;
    }
}