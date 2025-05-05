using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Services;

/// <summary>
/// Default implementation of the navigation service
/// </summary>
public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly EnhancedNavigationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    public NavigationService(
        ILogger<NavigationService> logger,
        IOptions<EnhancedNavigationOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new EnhancedNavigationOptions();
    }

    /// <inheritdoc/>
    public bool IsEnhancedNavigationEnabled => true;

    /// <inheritdoc/>
    public Task ScrollToTopAsync(ScrollBehavior? behavior = null, CancellationToken cancellationToken = default)
    {
        // This is implemented without JS interop for SSR compatibility
        _logger.LogInformation("Scrolling to top with behavior: {Behavior}", behavior ?? _options.Behavior);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ScrollToElementAsync(string elementId, ScrollBehavior? behavior = null, CancellationToken cancellationToken = default)
    {
        // This is implemented without JS interop for SSR compatibility
        _logger.LogInformation("Scrolling to element {ElementId} with behavior: {Behavior}", elementId, behavior ?? _options.Behavior);
        return Task.CompletedTask;
    }
}