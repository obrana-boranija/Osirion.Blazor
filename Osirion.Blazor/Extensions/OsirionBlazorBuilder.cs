using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Implementation of the IOsirionBlazorBuilder interface for fluent configuration.
/// </summary>
public class OsirionBlazorBuilder : IOsirionBlazorBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the OsirionBlazorBuilder class.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <exception cref="ArgumentNullException">Thrown if services is null.</exception>
    public OsirionBlazorBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddScrollToTop(Action<ScrollToTopManager> configure)
    {
        _services.AddScrollToTop(configure);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddScrollToTop(
        ButtonPosition position = ButtonPosition.BottomRight,
        ScrollBehavior behavior = ScrollBehavior.Smooth,
        int visibilityThreshold = 300,
        string? text = null)
    {
        _services.AddScrollToTop(position, behavior, visibilityThreshold, text);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddClarityTracker(Action<ClarityOptions> configure)
    {
        _services.AddClarityTracker(configure);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddClarityTracker(IConfiguration configuration)
    {
        _services.AddClarityTracker(configuration);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddMatomoTracker(Action<MatomoOptions> configure)
    {
        _services.AddMatomoTracker(configure);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddMatomoTracker(IConfiguration configuration)
    {
        _services.AddMatomoTracker(configuration);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddGitHubCms(Action<GitHubCmsOptions> configure)
    {
        _services.AddGitHubCms(configure);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddGitHubCms(IConfiguration configuration)
    {
        _services.AddGitHubCms(configuration);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddOsirionStyle(Action<OsirionStyleOptions> configure)
    {
        _services.AddOsirionStyle(configure);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddOsirionStyle(
        CssFramework framework,
        bool useStyles = true,
        string? customVariables = null)
    {
        _services.AddOsirionStyle(framework, useStyles, customVariables);
        return this;
    }

    /// <inheritdoc />
    public IOsirionBlazorBuilder AddOsirionStyle(IConfiguration configuration)
    {
        _services.AddOsirionStyle(configuration);
        return this;
    }
}