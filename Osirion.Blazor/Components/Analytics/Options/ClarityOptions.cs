namespace Osirion.Blazor.Components.Analytics.Options;

/// <summary>
/// Configuration options for Microsoft Clarity analytics
/// </summary>
public class ClarityOptions : TrackerBaseOptions
{
    /// <summary>
    /// Configuration section name for Microsoft Clarity options
    /// </summary>
    public const string Section = "Clarity";

    /// <summary>
    /// Gets or sets the URL of the Microsoft Clarity tracker script
    /// </summary>
    public override string? TrackerUrl { get; set; } = "https://www.clarity.ms/tag/";
}