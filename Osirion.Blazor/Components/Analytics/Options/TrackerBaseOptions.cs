namespace Osirion.Blazor.Components.Analytics.Options;

/// <summary>
/// Base options for analytics trackers
/// </summary>
public abstract class TrackerBaseOptions
{
    /// <summary>
    /// Gets or sets the URL of the tracker service
    /// </summary>
    public abstract string? TrackerUrl { get; set; }

    /// <summary>
    /// Gets or sets the site ID for the tracker
    /// </summary>
    public virtual string? SiteId { get; set; }

    /// <summary>
    /// Gets or sets whether tracking is enabled
    /// </summary>
    public virtual bool Track { get; set; } = true;
}