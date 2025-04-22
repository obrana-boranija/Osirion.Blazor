namespace Osirion.Blazor.Analytics;

/// <summary>
/// Interface for analytics providers
/// </summary>
public interface IAnalyticsProvider
{
    /// <summary>
    /// Gets the unique identifier for the provider
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Gets whether the provider is enabled
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Tracks a page view
    /// </summary>
    /// <param name="path">The path being viewed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks a custom event
    /// </summary>
    /// <param name="category">Event category</param>
    /// <param name="action">Event action</param>
    /// <param name="label">Optional event label</param>
    /// <param name="value">Optional event value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the tracking script for this provider
    /// </summary>
    /// <returns>The script HTML</returns>
    string GetScript();

    /// <summary>
    /// Gets whether the tracking script should render
    /// </summary>
    bool ShouldRender { get; }
}