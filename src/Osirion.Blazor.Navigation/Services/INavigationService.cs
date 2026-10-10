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
    /// Scrolls to the top of the page. Not implemented: the call only logs and never scrolls.
    /// </summary>
    /// <param name="behavior">The scroll behavior to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Obsolete("Does not scroll: it only writes a log entry. Use the ScrollToTop component, or call window.scrollTo through your own IJSRuntime wrapper.")]
    Task ScrollToTopAsync(ScrollBehavior? behavior = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scrolls to a specific element. Not implemented: the call only logs and never scrolls.
    /// </summary>
    /// <param name="elementId">The ID of the element to scroll to</param>
    /// <param name="behavior">The scroll behavior to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Obsolete("Does not scroll: it only writes a log entry. Use an anchor link to the element id, or call scrollIntoView through your own IJSRuntime wrapper.")]
    Task ScrollToElementAsync(string elementId, ScrollBehavior? behavior = null, CancellationToken cancellationToken = default);
}
