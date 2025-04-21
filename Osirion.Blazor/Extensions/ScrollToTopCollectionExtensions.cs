using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for configuring ScrollToTop services in the DI container.
/// </summary>
public static class ScrollToTopServiceCollectionExtensions
{
    /// <summary>
    /// Adds ScrollToTop service to the service collection with configuration from settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration containing ScrollToTop settings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddScrollToTop(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            return services;
        }

        // Get options from configuration
        var options = new ScrollToTopOptions();
        configuration.GetSection(ScrollToTopOptions.Section).Bind(options);

        // Create and configure manager from options
        var manager = new ScrollToTopManager
        {
            IsEnabled = true,
            Position = options.Position,
            Behavior = options.Behavior,
            VisibilityThreshold = options.VisibilityThreshold,
            Text = options.Text,
            Title = options.Title,
            CssClass = options.CssClass,
            CustomIcon = options.CustomIcon
        };

        // Register the manager as singleton
        services.AddSingleton(manager);

        // Also register the options for components that need it
        services.Configure<ScrollToTopOptions>(
            configuration.GetSection(ScrollToTopOptions.Section));

        return services;
    }

    /// <summary>
    /// Adds ScrollToTop service to the service collection with common parameters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="position">Position of the ScrollToTop button.</param>
    /// <param name="behavior">Scroll behavior.</param>
    /// <param name="visibilityThreshold">Visibility threshold in pixels.</param>
    /// <param name="text">Optional text to display.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddScrollToTop(
        this IServiceCollection services,
        ButtonPosition position = ButtonPosition.BottomRight,
        ScrollBehavior behavior = ScrollBehavior.Smooth,
        int visibilityThreshold = 300,
        string? text = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register the ScrollToTopManager as a singleton
        services.AddSingleton(new ScrollToTopManager
        {
            IsEnabled = true,
            Position = position,
            Behavior = behavior,
            VisibilityThreshold = visibilityThreshold,
            Text = text
        });

        // Also register the corresponding options
        services.Configure<ScrollToTopOptions>(options =>
        {
            options.Position = position;
            options.Behavior = behavior;
            options.VisibilityThreshold = visibilityThreshold;
            options.Text = text;
        });

        return services;
    }

    /// <summary>
    /// Adds ScrollToTop service to the service collection with detailed configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure the ScrollToTop manager.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddScrollToTop(
        this IServiceCollection services,
        Action<ScrollToTopManager> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            return services;
        }

        // Create a new manager instance with default settings
        var manager = new ScrollToTopManager { IsEnabled = true };

        // Apply custom configuration
        configure.Invoke(manager);

        // Register as singleton
        services.AddSingleton(manager);

        // Also register corresponding options
        services.Configure<ScrollToTopOptions>(options =>
        {
            options.Position = manager.Position;
            options.Behavior = manager.Behavior;
            options.VisibilityThreshold = manager.VisibilityThreshold;
            options.Text = manager.Text;
            options.Title = manager.Title;
            options.CssClass = manager.CssClass;
            options.CustomIcon = manager.CustomIcon;
        });

        return services;
    }

    /// <summary>
    /// Adds ScrollToTop options without enabling the global ScrollToTop service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration containing ScrollToTop settings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddScrollToTopOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            return services;
        }

        // Configure options from the settings
        services.Configure<ScrollToTopOptions>(
            configuration.GetSection(ScrollToTopOptions.Section));

        return services;
    }
}