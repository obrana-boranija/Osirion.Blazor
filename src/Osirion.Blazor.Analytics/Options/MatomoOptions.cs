namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Matomo analytics
/// </summary>
public class MatomoOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "Osirion:Analytics:Matomo";

    /// <summary>
    /// Gets or sets the Matomo tracker URL
    /// </summary>
    public string TrackerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to track links
    /// </summary>
    public bool TrackLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to track downloads. It has no effect: Matomo reports downloads and outlinks together when link
    /// tracking is on (<see cref="TrackLinks"/>), and its tracker has no documented switch for downloads alone.
    /// </summary>
    [Obsolete("Has no effect: Matomo tracks downloads together with outlinks when TrackLinks is on and has no documented switch for downloads alone.")]
    public bool TrackDownloads { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to require user consent before tracking
    /// </summary>
    public bool RequireConsent { get; set; } = false;
}
