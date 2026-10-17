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
    /// Gets or sets whether to track user attributes. It has no effect: Clarity has no install-time setting for it. Send
    /// user data from your own code with <c>clarity("identify", ...)</c> or <c>clarity("set", key, value)</c>.
    /// </summary>
    [Obsolete("Has no effect: Clarity has no install-time setting for user attributes. Call clarity(\"identify\", ...) or clarity(\"set\", key, value) from your own code.")]
    public bool TrackUserAttributes { get; set; } = true;
}
