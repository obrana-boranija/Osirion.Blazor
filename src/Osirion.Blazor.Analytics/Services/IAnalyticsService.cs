namespace Osirion.Blazor.Analytics.Services;

/// <summary>
/// Service for managing multiple analytics providers
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets all registered providers
    /// </summary>
    IReadOnlyList<IAnalyticsProvider> Providers { get; }

    /// <summary>
    /// Tracks a page view across all enabled providers
    /// </summary>
    Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks an event across all enabled providers
    /// </summary>
    Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific provider by ID
    /// </summary>
    IAnalyticsProvider? GetProvider(string providerId);
}