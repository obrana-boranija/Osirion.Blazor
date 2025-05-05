using Microsoft.Extensions.Options;
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
    public bool IsEnabled => _options.Enabled && !string.IsNullOrEmpty(_options.MeasurementId);

    /// <inheritdoc/>
    public bool ShouldRender => IsEnabled;

    /// <inheritdoc/>
    public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        // GA4 tracks page views automatically based on configuration
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
    {
        // GA4 requires JavaScript interop for custom events
        // This would be handled by injected script
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string GetScript()
    {
        var config = BuildConfiguration();

        var sb = new StringBuilder();

        // Add the Global Site Tag (gtag.js)
        sb.AppendLine($@"
            <script async src=""https://www.googletagmanager.com/gtag/js?id={_options.MeasurementId}""></script>
            <script>
                window.dataLayer = window.dataLayer || [];
                function gtag(){{dataLayer.push(arguments);}}
                gtag('js', new Date());");

        // Add debug mode if enabled
        if (_options.DebugMode)
        {
            sb.AppendLine(@"
                window.ga_debug = {debug_mode: true};");
        }

        // Add main configuration
        sb.AppendLine($@"
                gtag('config', '{_options.MeasurementId}', {JsonSerializer.Serialize(config)});");

        // Add enhanced link attribution if enabled
        if (_options.LinkAttribution)
        {
            sb.AppendLine(@"
                gtag('require', 'linkid');");
        }

        // Add outbound link tracking if enabled
        if (_options.TrackOutboundLinks)
        {
            sb.AppendLine(@"
                // Outbound Link Tracking
                var trackOutboundLink = function(url) {
                    gtag('event', 'click', {
                        'event_category': 'outbound',
                        'event_label': url,
                        'transport_type': 'beacon',
                        'event_callback': function(){document.location = url;}
                    });
                }
                
                // Automatically track outbound links
                document.addEventListener('click', function(e) {
                    var el = e.target;
                    while (el && el.tagName !== 'A') {
                        el = el.parentElement;
                    }
                    if (el && el.href && el.hostname !== window.location.hostname) {
                        e.preventDefault();
                        trackOutboundLink(el.href);
                    }
                });");
        }

        sb.AppendLine(@"
            </script>");

        return sb.ToString();
    }

    private Dictionary<string, object> BuildConfiguration()
    {
        var config = new Dictionary<string, object>();

        if (_options.AnonymizeIp)
        {
            config["anonymize_ip"] = true;
        }

        if (!string.IsNullOrEmpty(_options.CookieFlags))
        {
            config["cookie_flags"] = _options.CookieFlags;
        }

        if (!string.IsNullOrEmpty(_options.CookieDomain))
        {
            config["cookie_domain"] = _options.CookieDomain;
        }

        if (_options.CookieExpires.HasValue)
        {
            config["cookie_expires"] = _options.CookieExpires.Value;
        }

        if (_options.SendPageView)
        {
            config["send_page_view"] = true;
        }

        config["transport_type"] = _options.TransportType;

        if (_options.RestrictDataProcessing)
        {
            config["restricted_data_processing"] = true;
        }

        // Add any custom configuration parameters
        if (_options.ConfigParameters != null)
        {
            foreach (var param in _options.ConfigParameters)
            {
                config[param.Key] = param.Value;
            }
        }

        return config;
    }
}