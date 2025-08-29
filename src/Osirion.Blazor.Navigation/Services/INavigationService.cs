using Osirion.Blazor.Components;

namespace Osirion.Blazor.Navigation.Services;

/// <summary>
/// Service for managing navigation functionality
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets whether enhanced navigation is enabled
    /// </summary>
    bool IsEnhancedNavigationEnabled { get; }

    /// <summary>
    /// Scrolls to the top of the page
    /// </summary>
    /// <param name="behavior">The scroll behavior to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ScrollToTopAsync(ScrollBehavior? behavior = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scrolls to a specific element
    /// </summary>
    /// <param name="elementId">The ID of the element to scroll to</param>
    /// <param name="behavior">The scroll behavior to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ScrollToElementAsync(string elementId, ScrollBehavior? behavior = null, CancellationToken cancellationToken = default);
}