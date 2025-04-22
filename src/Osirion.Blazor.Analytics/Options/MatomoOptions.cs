namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Matomo analytics
/// </summary>
public class MatomoOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Matomo";

    /// <summary>
    /// Gets or sets the Matomo tracker URL
    /// </summary>
    public string TrackerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to track links
    /// </summary>
    public bool TrackLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to track downloads
    /// </summary>
    public bool TrackDownloads { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to require user consent before tracking
    /// </summary>
    public bool RequireConsent { get; set; } = false;
}