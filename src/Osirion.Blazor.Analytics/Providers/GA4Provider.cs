using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Options;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Analytics.Providers;

/// <summary>
/// Google Analytics 4 provider
/// </summary>
public class GA4Provider : IAnalyticsProvider
{
    private readonly GA4Options _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GA4Provider"/> class.
    /// </summary>
    public GA4Provider(IOptions<GA4Options> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public string ProviderId => "ga4";

    /// <inheritdoc/>
    public bool IsEnabled => CanRender(_options);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // GA4 tracks page views automatically based on configuration; for enhanced navigation
        // it relies on Enhanced Measurement ("Page changes based on browser history events").
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // GA4 requires JavaScript interop for custom events
        // This would be handled by injected script
        return Task.CompletedTask;
    }

    /// <summary>Creates a copy of the configured options that a tracker can adjust for one render.</summary>
    public GA4Options CreateOptionsCopy() => (GA4Options)_options.ShallowCopy();

    /// <summary>
    /// Gets whether a script can be rendered for <paramref name="options"/>. A measurement ID that is
    /// not a Google tag ID (<c>G-XXXXXXXX</c>) disables rendering instead of reaching the page.
    /// </summary>
    public bool CanRender(GA4Options options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Enabled && AnalyticsFormats.IsGoogleTagId(options.MeasurementId);
    }

    /// <inheritdoc/>
    public string GetScript() => GetScript(_options, AnalyticsScriptContext.Default);

    /// <inheritdoc/>
    public string GetScript(AnalyticsScriptContext context) => GetScript(_options, context);

    /// <summary>Builds the script for <paramref name="options"/>.</summary>
    public string GetScript(GA4Options options, AnalyticsScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);

        var config = BuildConfiguration(options);
        var measurementId = ScriptLiteral.Js(options.MeasurementId);
        var scriptSource = ScriptLiteral.Attribute(
            "https://www.googletagmanager.com/gtag/js?id=" + Uri.EscapeDataString(options.MeasurementId));

        var sb = new StringBuilder();

        // Add the Global Site Tag (gtag.js)
        sb.AppendLine($@"
            <script async src=""{scriptSource}""></script>
            <script>
                window.dataLayer = window.dataLayer || [];
                function gtag(){{dataLayer.push(arguments);}}");

        // Consent Mode defaults must precede the first config call. The script is only built after the
        // visitor consented to analytics, so analytics storage is granted right after the defaults;
        // advertising signals stay denied because this gate says nothing about marketing consent.
        if (context.ConsentGateActive)
        {
            sb.AppendLine(@"
                gtag('consent', 'default', {'ad_storage': 'denied', 'ad_user_data': 'denied', 'ad_personalization': 'denied', 'analytics_storage': 'denied'});
                gtag('consent', 'update', {'analytics_storage': 'granted'});");
        }

        sb.AppendLine(@"
                gtag('js', new Date());");

        // Add main configuration
        sb.AppendLine($@"
                gtag('config', {measurementId}, {JsonSerializer.Serialize(config)});");

        // Outbound link tracking. It only reports: the browser follows the link itself, so a blocked
        // or failing gtag can never break navigation, and target, modified clicks and non-http(s)
        // schemes (mailto:, tel:) are left alone.
        if (options.TrackOutboundLinks)
        {
            sb.AppendLine(@"
                document.addEventListener('click', function (e) {
                    if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) { return; }
                    var el = e.target && e.target.closest ? e.target.closest('a[href]') : null;
                    if (!el || el.hasAttribute('download')) { return; }
                    var target = el.getAttribute('target');
                    if (target && target !== '_self') { return; }
                    if (el.protocol !== 'http:' && el.protocol !== 'https:') { return; }
                    if (el.hostname === window.location.hostname) { return; }
                    gtag('event', 'click', {
                        'event_category': 'outbound',
                        'event_label': el.href,
                        'transport_type': 'beacon'
                    });
                });");
        }

        sb.AppendLine(@"
            </script>");

        return sb.ToString();
    }

    private static Dictionary<string, object> BuildConfiguration(GA4Options options)
    {
        var config = new Dictionary<string, object>();

        if (options.AnonymizeIp)
        {
            config["anonymize_ip"] = true;
        }

        if (!string.IsNullOrWhiteSpace(options.CookieFlags))
        {
            config["cookie_flags"] = options.CookieFlags;
        }

        if (!string.IsNullOrWhiteSpace(options.CookieDomain))
        {
            config["cookie_domain"] = options.CookieDomain;
        }

        if (options.CookieExpires.HasValue)
        {
            config["cookie_expires"] = options.CookieExpires.Value;
        }

        // send_page_view defaults to true in gtag, so false has to be written explicitly.
        config["send_page_view"] = options.SendPageView;

        config["transport_type"] = options.TransportType;

        if (options.RestrictDataProcessing)
        {
            config["restricted_data_processing"] = true;
        }

        // DebugView documents debug_mode in the config call. Only true is written: Google says leaving the parameter
        // out turns debug mode off, and that false does not.
        if (options.DebugMode)
        {
            config["debug_mode"] = true;
        }

        // Add any custom configuration parameters
        if (options.ConfigParameters is not null)
        {
            foreach (var param in options.ConfigParameters)
            {
                config[param.Key] = param.Value;
            }
        }

        return config;
    }
}
