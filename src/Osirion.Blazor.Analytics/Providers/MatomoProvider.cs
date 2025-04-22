using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Matomo analytics provider
/// </summary>
public class MatomoProvider : IAnalyticsProvider
{
    private readonly MatomoOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatomoProvider"/> class.
    /// </summary>
    public MatomoProvider(IOptions<MatomoOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "matomo";

    /// <inheritdoc/>
    public bool IsEnabled => _options.Enabled && !string.IsNullOrEmpty(_options.SiteId) && !string.IsNullOrEmpty(_options.TrackerUrl);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Matomo tracks page views automatically
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Matomo requires JavaScript interop for custom events
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string GetScript()
    {
        var requireConsentScript = _options.RequireConsent ?
            "_paq.push(['requireConsent']);" : string.Empty;
        var trackLinksScript = _options.TrackLinks ?
            "_paq.push(['enableLinkTracking']);" : string.Empty;

        return $@"
            <script>
                var _paq = window._paq = window._paq || [];
                {requireConsentScript}
                _paq.push(['trackPageView']);
                {trackLinksScript}
                (function () {{
                    var u = '{_options.TrackerUrl}';
                    _paq.push(['setTrackerUrl', u + 'matomo.php']);
                    _paq.push(['setSiteId', '{_options.SiteId}']);
                    var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
                    g.async = true; g.src = u + 'matomo.js'; s.parentNode.insertBefore(g, s);
                }})();
            </script>
        ";
    }
}