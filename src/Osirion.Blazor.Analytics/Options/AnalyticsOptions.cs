namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Base options for analytics providers
/// </summary>
public abstract class AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Analytics";

    /// <summary>
    /// Gets or sets the site ID or tracking ID
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// Gets or sets whether the provider is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to automatically track page views
    /// </summary>
    public bool AutoTrackPageViews { get; set; } = true;
}