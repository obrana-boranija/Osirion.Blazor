namespace Osirion.Blazor.Components.Analytics.Options;

/// <summary>
/// Configuration options for Matomo analytics
/// </summary>
public class MatomoOptions : TrackerBaseOptions
{
    /// <summary>
    /// Configuration section name for Matomo options
    /// </summary>
    public const string Section = "Matomo";

    /// <summary>
    /// Gets or sets the URL of the Matomo tracker
    /// </summary>
    public override string? TrackerUrl { get; set; } = "//analytics.tridesetri.com/";
}