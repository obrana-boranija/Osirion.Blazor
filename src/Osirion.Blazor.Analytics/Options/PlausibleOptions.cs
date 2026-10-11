namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Plausible Analytics.
/// </summary>
/// <remarks>
/// Plausible gives every site its own script, shown under Site installation in the site settings. The script reads
/// that site's tracking settings, so the optional switches below are left out of the markup unless they are set,
/// and only then override what the dashboard configured. <see cref="AnalyticsOptions.SiteId"/> is not used.
/// </remarks>
public class PlausibleOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "Osirion:Analytics:Plausible";

    /// <summary>
    /// Gets or sets the site's script, such as <c>https://plausible.io/js/pa-XXXXX.js</c>, or the path a proxy
    /// serves it from, such as <c>/js/script.js</c>. Nothing renders while it is empty.
    /// </summary>
    public string ScriptUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets where events are sent, for a proxy such as <c>/api/event</c>. Defaults to Plausible's endpoint.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>Gets or sets whether page paths after <c>#</c> are tracked as separate pages.</summary>
    public bool? HashBasedRouting { get; set; }

    /// <summary>Gets or sets whether clicks on links to other sites are tracked.</summary>
    public bool? OutboundLinks { get; set; }

    /// <summary>Gets or sets whether file downloads are tracked.</summary>
    public bool? FileDownloads { get; set; }

    /// <summary>Gets or sets whether form submissions are tracked.</summary>
    public bool? FormSubmissions { get; set; }

    /// <summary>Gets or sets whether visits to <c>localhost</c> are tracked, which Plausible skips by default.</summary>
    public bool? CaptureOnLocalhost { get; set; }
}
