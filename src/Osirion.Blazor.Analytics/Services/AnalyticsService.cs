using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Analytics.Services;

/// <summary>
/// Default implementation of the analytics service
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly IEnumerable<IAnalyticsProvider> _providers;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsService"/> class.
    /// </summary>
    public AnalyticsService(
        IEnumerable<IAnalyticsProvider> providers,
        ILogger<AnalyticsService> logger)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public IReadOnlyList<IAnalyticsProvider> Providers => _providers.ToList().AsReadOnly();

    /// <inheritdoc/>
    public async Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        foreach (var provider in _providers.Where(p => p.IsEnabled))
        {
            try
            {
                await provider.TrackPageViewAsync(path, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking page view with provider {ProviderId}", provider.ProviderId);
            }
        }
    }

    /// <inheritdoc/>
    public async Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        foreach (var provider in _providers.Where(p => p.IsEnabled))
        {
            try
            {
                await provider.TrackEventAsync(category, action, label, value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking event with provider {ProviderId}", provider.ProviderId);
            }
        }
    }

    /// <inheritdoc/>
    public IAnalyticsProvider? GetProvider(string providerId)
    {
        return _providers.FirstOrDefault(p => p.ProviderId.Equals(providerId, StringComparison.OrdinalIgnoreCase));
    }
}