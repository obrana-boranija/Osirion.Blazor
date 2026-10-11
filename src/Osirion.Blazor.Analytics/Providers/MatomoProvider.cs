using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Internal;
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
    public bool IsEnabled => CanRender(_options);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // Matomo tracks the first page view from the script; later enhanced navigations are
        // reported by the osirion-analytics.js module.
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // Matomo requires JavaScript interop for custom events
        return Task.CompletedTask;
    }

    /// <summary>Creates a copy of the configured options that a tracker can adjust for one render.</summary>
    public MatomoOptions CreateOptionsCopy() => (MatomoOptions)_options.ShallowCopy();

    /// <summary>Gets whether a script can be rendered for <paramref name="options"/>.</summary>
    public bool CanRender(MatomoOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Enabled && !string.IsNullOrWhiteSpace(options.SiteId)
            && AnalyticsFormats.IsHttpOrProtocolRelativeUrl(options.TrackerUrl);
    }

    /// <inheritdoc/>
    public string GetScript() => GetScript(_options, AnalyticsScriptContext.Default);

    /// <inheritdoc/>
    public string GetScript(AnalyticsScriptContext context) => GetScript(_options, context);

    /// <summary>Builds the script for <paramref name="options"/>.</summary>
    /// <remarks>
    /// When the consent gate is on the script only exists after the visitor consented, so Matomo's own
    /// <c>requireConsent</c> is not emitted. Without the gate, <see cref="MatomoOptions.RequireConsent"/>
    /// keeps Matomo silent until the host page pushes <c>['setConsentGiven']</c> or
    /// <c>['rememberConsentGiven']</c> onto <c>_paq</c>.
    /// </remarks>
    public string GetScript(MatomoOptions options, AnalyticsScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);

        var requireConsentScript = options.RequireConsent && !context.ConsentGateActive ?
            "_paq.push(['requireConsent']);" : string.Empty;
        var trackLinksScript = options.TrackLinks ?
            "_paq.push(['enableLinkTracking']);" : string.Empty;
        var pageViewModule = options.AutoTrackPageViews
            ? PageViewModuleTag.Build("matomo")
            : string.Empty;

        return $@"
            <script>
                var _paq = window._paq = window._paq || [];
                {requireConsentScript}
                _paq.push(['trackPageView']);
                {trackLinksScript}
                (function () {{
                    var u = {ScriptLiteral.Js(options.TrackerUrl)};
                    _paq.push(['setTrackerUrl', u + 'matomo.php']);
                    _paq.push(['setSiteId', {ScriptLiteral.Js(options.SiteId)}]);
                    var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
                    g.async = true; g.src = u + 'matomo.js'; s.parentNode.insertBefore(g, s);
                }})();
            </script>
            {pageViewModule}
        ";
    }
}
