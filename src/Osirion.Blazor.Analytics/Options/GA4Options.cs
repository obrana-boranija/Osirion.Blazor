namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Google Analytics 4
/// </summary>
public class GA4Options : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "Osirion:Analytics:GA4";

    /// <summary>
    /// Gets or sets the GA4 measurement ID (format: G-XXXXXXXXXX)
    /// </summary>
    public string MeasurementId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to anonymize IP addresses
    /// </summary>
    public bool AnonymizeIp { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable enhanced link attribution
    /// </summary>
    public bool LinkAttribution { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable debug mode
    /// </summary>
    public bool DebugMode { get; set; } = false;

    /// <summary>
    /// Gets or sets custom cookie flags
    /// </summary>
    public string? CookieFlags { get; set; }

    /// <summary>
    /// Gets or sets custom config parameters
    /// </summary>
    public Dictionary<string, object>? ConfigParameters { get; set; }

    /// <summary>
    /// Gets or sets whether to send page view on initialization
    /// </summary>
    public bool SendPageView { get; set; } = true;

    /// <summary>
    /// Gets or sets the transport type (beacon or xhr)
    /// </summary>
    public string TransportType { get; set; } = "beacon";

    /// <summary>
    /// Gets or sets whether to track outbound links
    /// </summary>
    public bool TrackOutboundLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets the cookie domain
    /// </summary>
    public string? CookieDomain { get; set; }

    /// <summary>
    /// Gets or sets the cookie expiration in seconds
    /// </summary>
    public int? CookieExpires { get; set; }

    /// <summary>
    /// Gets or sets whether to restrict data processing (GDPR compliance)
    /// </summary>
    public bool RestrictDataProcessing { get; set; } = false;
}