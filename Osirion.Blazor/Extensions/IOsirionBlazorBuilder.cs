using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Builder interface for configuring Osirion.Blazor services using a fluent API.
/// </summary>
public interface IOsirionBlazorBuilder
{
    /// <summary>
    /// Adds ScrollToTop functionality with detailed configuration.
    /// </summary>
    /// <param name="configure">Action to configure ScrollToTop options.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddScrollToTop(Action<ScrollToTopManager> configure);

    /// <summary>
    /// Adds ScrollToTop functionality with common configuration parameters.
    /// </summary>
    /// <param name="position">Position of the button on screen.</param>
    /// <param name="behavior">Scroll animation behavior.</param>
    /// <param name="visibilityThreshold">Scroll threshold in pixels to show the button.</param>
    /// <param name="text">Optional text to display on the button.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddScrollToTop(
        ButtonPosition position = ButtonPosition.BottomRight,
        ScrollBehavior behavior = ScrollBehavior.Smooth,
        int visibilityThreshold = 300,
        string? text = null);

    /// <summary>
    /// Adds Microsoft Clarity analytics tracking.
    /// </summary>
    /// <param name="configure">Action to configure Clarity options.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddClarityTracker(Action<ClarityOptions> configure);

    /// <summary>
    /// Adds Microsoft Clarity analytics tracking from configuration.
    /// </summary>
    /// <param name="configuration">Configuration containing Clarity settings.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddClarityTracker(IConfiguration configuration);

    /// <summary>
    /// Adds Matomo analytics tracking.
    /// </summary>
    /// <param name="configure">Action to configure Matomo options.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddMatomoTracker(Action<MatomoOptions> configure);

    /// <summary>
    /// Adds Matomo analytics tracking from configuration.
    /// </summary>
    /// <param name="configuration">Configuration containing Matomo settings.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddMatomoTracker(IConfiguration configuration);

    /// <summary>
    /// Adds GitHub CMS functionality.
    /// </summary>
    /// <param name="configure">Action to configure GitHub CMS options.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddGitHubCms(Action<GitHubCmsOptions> configure);

    /// <summary>
    /// Adds GitHub CMS functionality from configuration.
    /// </summary>
    /// <param name="configuration">Configuration containing GitHub CMS settings.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddGitHubCms(IConfiguration configuration);

    /// <summary>
    /// Adds Osirion styling configuration.
    /// </summary>
    /// <param name="configure">Action to configure styling options.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddOsirionStyle(Action<OsirionStyleOptions> configure);

    /// <summary>
    /// Adds Osirion styling with framework integration.
    /// </summary>
    /// <param name="framework">The CSS framework to integrate with.</param>
    /// <param name="useStyles">Whether to use default styles.</param>
    /// <param name="customVariables">Optional custom CSS variables.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddOsirionStyle(
        CssFramework framework,
        bool useStyles = true,
        string? customVariables = null);

    /// <summary>
    /// Adds Osirion styling from configuration.
    /// </summary>
    /// <param name="configuration">Configuration containing styling settings.</param>
    /// <returns>The builder for chaining.</returns>
    IOsirionBlazorBuilder AddOsirionStyle(IConfiguration configuration);
}