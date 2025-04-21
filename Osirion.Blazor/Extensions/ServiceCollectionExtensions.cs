using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure Osirion.Blazor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register the base services
        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor services with configuration from the specified <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration instance to bind from.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor services with customized options using a builder pattern.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureAction">The action to configure Osirion services.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services, Action<OsirionBlazorBuilder> configureAction)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureAction == null)
        {
            throw new ArgumentNullException(nameof(configureAction));
        }

        // Create a builder for fluent configuration
        var builder = new OsirionBlazorBuilder(services);

        // Configure using the provided action
        configureAction(builder);

        return services;
    }
}

/// <summary>
/// Builder class for configuring Osirion.Blazor services using a fluent API.
/// </summary>
public class OsirionBlazorBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="OsirionBlazorBuilder"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    public OsirionBlazorBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Adds global ScrollToTop functionality.
    /// </summary>
    /// <param name="configureAction">The action to configure the ScrollToTop manager.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddScrollToTop(Action<ScrollToTopManager>? configureAction = null)
    {
        if (configureAction != null)
        {
            _services.AddScrollToTop(configureAction);
        }
        else
        {
            _services.AddScrollToTop();
        }

        return this;
    }

    /// <summary>
    /// Adds global ScrollToTop functionality with specific settings.
    /// </summary>
    /// <param name="position">The position of the ScrollToTop button.</param>
    /// <param name="behavior">The scroll behavior.</param>
    /// <param name="visibilityThreshold">The visibility threshold in pixels.</param>
    /// <param name="text">Optional text to display.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddScrollToTop(
        ButtonPosition position = ButtonPosition.BottomRight,
        ScrollBehavior behavior = ScrollBehavior.Smooth,
        int visibilityThreshold = 300,
        string? text = null)
    {
        _services.AddScrollToTop(position, behavior, visibilityThreshold, text);
        return this;
    }

    /// <summary>
    /// Adds Microsoft Clarity analytics tracking.
    /// </summary>
    /// <param name="configureAction">The action to configure Clarity options.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddClarityTracker(Action<ClarityOptions> configureAction)
    {
        _services.AddClarityTracker(configureAction);
        return this;
    }

    /// <summary>
    /// Adds Microsoft Clarity analytics tracking with configuration.
    /// </summary>
    /// <param name="configuration">The configuration containing Clarity settings.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddClarityTracker(IConfiguration configuration)
    {
        _services.AddClarityTracker(configuration);
        return this;
    }

    /// <summary>
    /// Adds Matomo analytics tracking.
    /// </summary>
    /// <param name="configureAction">The action to configure Matomo options.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddMatomoTracker(Action<MatomoOptions> configureAction)
    {
        _services.AddMatomoTracker(configureAction);
        return this;
    }

    /// <summary>
    /// Adds Matomo analytics tracking with configuration.
    /// </summary>
    /// <param name="configuration">The configuration containing Matomo settings.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddMatomoTracker(IConfiguration configuration)
    {
        _services.AddMatomoTracker(configuration);
        return this;
    }

    /// <summary>
    /// Adds GitHub CMS service.
    /// </summary>
    /// <param name="configureAction">The action to configure GitHub CMS options.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddGitHubCms(Action<GitHubCmsOptions> configureAction)
    {
        _services.AddGitHubCms(configureAction);
        return this;
    }

    /// <summary>
    /// Adds GitHub CMS service with configuration.
    /// </summary>
    /// <param name="configuration">The configuration containing GitHub CMS settings.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddGitHubCms(IConfiguration configuration)
    {
        _services.AddGitHubCms(configuration);
        return this;
    }

    /// <summary>
    /// Adds all services from configuration using conventional section names.
    /// </summary>
    /// <param name="configuration">The configuration containing all settings.</param>
    /// <param name="enableGlobalScrollToTop">Whether to enable global ScrollToTop.</param>
    /// <param name="enableClarityTracker">Whether to enable Clarity tracker.</param>
    /// <param name="enableMatomoTracker">Whether to enable Matomo tracker.</param>
    /// <param name="enableGitHubCms">Whether to enable GitHub CMS.</param>
    /// <returns>The builder for chaining.</returns>
    public OsirionBlazorBuilder AddAllServices(
        IConfiguration configuration,
        bool enableGlobalScrollToTop = true,
        bool enableClarityTracker = true,
        bool enableMatomoTracker = true,
        bool enableGitHubCms = true)
    {
        if (enableGlobalScrollToTop && configuration.GetSection(ScrollToTopOptions.Section).Exists())
        {
            AddScrollToTop(manager =>
            {
                var options = new ScrollToTopOptions();
                configuration.GetSection(ScrollToTopOptions.Section).Bind(options);

                manager.Position = options.Position;
                manager.Behavior = options.Behavior;
                manager.VisibilityThreshold = options.VisibilityThreshold;
                manager.Text = options.Text;
                manager.Title = options.Title;
                manager.CssClass = options.CssClass;
                manager.CustomIcon = options.CustomIcon;
            });
        }

        if (enableClarityTracker && configuration.GetSection(ClarityOptions.Section).Exists())
        {
            AddClarityTracker(configuration);
        }

        if (enableMatomoTracker && configuration.GetSection(MatomoOptions.Section).Exists())
        {
            AddMatomoTracker(configuration);
        }

        if (enableGitHubCms && configuration.GetSection(GitHubCmsOptions.Section).Exists())
        {
            AddGitHubCms(configuration);
        }

        return this;
    }
}