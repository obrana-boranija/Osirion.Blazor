namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Yandex Metrica analytics
/// </summary>
public class YandexMetricaOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "YandexMetrica";

    /// <summary>
    /// Gets or sets the Yandex Metrica counter ID
    /// </summary>
    public string CounterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to track clicks on the site's outbound links
    /// </summary>
    public bool TrackLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to accurately track bounce rate
    /// </summary>
    public bool AccurateTrackBounce { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable session replay
    /// </summary>
    public bool WebVisor { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to create a map of clicks
    /// </summary>
    public bool ClickMap { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to collect data for all pages, not just pages with the counter code
    /// </summary>
    public bool? TrackHash { get; set; } = null;

    /// <summary>
    /// Gets or sets whether to automatically send the hash part of the URL
    /// </summary>
    public bool HashTracking { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to disable automatic sending of page view data
    /// </summary>
    public bool DeferLoad { get; set; } = false;

    /// <summary>
    /// Gets or sets the alternative CDN domain
    /// </summary>
    public string? AlternativeCdn { get; set; }

    /// <summary>
    /// Gets or sets custom parameters to track
    /// </summary>
    public Dictionary<string, object>? Params { get; set; }

    /// <summary>
    /// Gets or sets user parameters for tracking
    /// </summary>
    public Dictionary<string, object>? UserParams { get; set; }

    /// <summary>
    /// Gets or sets whether to enable e-commerce data layer
    /// </summary>
    public bool EcommerceEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the container name for e-commerce data layer
    /// </summary>
    public string EcommerceContainerName { get; set; } = "dataLayer";
}