namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Umami analytics. <see cref="AnalyticsOptions.SiteId"/> is the website ID.
/// </summary>
public class UmamiOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "Osirion:Analytics:Umami";

    /// <summary>
    /// Gets or sets the tracker script. Defaults to Umami Cloud; a self-hosted instance serves it from
    /// <c>https://your-umami-host/script.js</c>.
    /// </summary>
    public string ScriptUrl { get; set; } = "https://cloud.umami.is/script.js";

    /// <summary>
    /// Gets or sets where data is sent (<c>data-host-url</c>). Defaults to the host the script came from.
    /// </summary>
    public string? HostUrl { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated list of host names the tracker runs on (<c>data-domains</c>), for example
    /// <c>example.com,www.example.com</c>. Other hosts, such as a staging site, are not tracked.
    /// </summary>
    public string? Domains { get; set; }

    /// <summary>Gets or sets a tag that groups this site's events for filtering (<c>data-tag</c>).</summary>
    public string? Tag { get; set; }

    /// <summary>Gets or sets whether URL query strings are left out of the collected data.</summary>
    public bool ExcludeSearch { get; set; }

    /// <summary>Gets or sets whether URL fragments are left out of the collected data.</summary>
    public bool ExcludeHash { get; set; }

    /// <summary>Gets or sets whether visitors with the browser's Do Not Track setting are not tracked.</summary>
    public bool DoNotTrack { get; set; }
}
