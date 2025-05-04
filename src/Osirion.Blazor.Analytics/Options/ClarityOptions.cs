namespace Osirion.Blazor.Analytics.Options;

/// <summary>
/// Configuration options for Microsoft Clarity
/// </summary>
public class ClarityOptions : AnalyticsOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public new const string Section = "Osirion:Analytics:Clarity";

    /// <summary>
    /// Gets or sets the Clarity tracker URL
    /// </summary>
    public string TrackerUrl { get; set; } = "https://www.clarity.ms/tag/";

    /// <summary>
    /// Gets or sets whether to track user attributes
    /// </summary>
    public bool TrackUserAttributes { get; set; } = true;
}
