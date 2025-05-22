using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// HeroSection component for displaying hero content with optional background image and buttons
/// </summary>
public partial class HeroSection
{
    /// <summary>
    /// Gets or sets the hero title
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the hero subtitle
    /// </summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets the background image URL
    /// </summary>
    [Parameter]
    public string? BackgroundImage { get; set; }

    /// <summary>
    /// Gets or sets the hero alignment: "left", "center", "right"
    /// </summary>
    [Parameter]
    public string Alignment { get; set; } = "center";

    /// <summary>
    /// Gets or sets whether to show buttons
    /// </summary>
    [Parameter]
    public bool ShowButtons { get; set; } = true;

    /// <summary>
    /// Gets or sets the primary button text
    /// </summary>
    [Parameter]
    public string? PrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the primary button URL
    /// </summary>
    [Parameter]
    public string? PrimaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the secondary button text
    /// </summary>
    [Parameter]
    public string? SecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the secondary button URL
    /// </summary>
    [Parameter]
    public string? SecondaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the background color (fallback if no image)
    /// </summary>
    [Parameter]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text color override
    /// </summary>
    [Parameter]
    public string? TextColor { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of the hero section
    /// </summary>
    [Parameter]
    public string MinHeight { get; set; } = "60vh";

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the CSS class for the hero section
    /// </summary>
    private string GetHeroSectionClass()
    {
        var classes = new List<string> { "osirion-hero-section" };

        classes.Add($"osirion-hero-align-{Alignment}");

        if (!string.IsNullOrEmpty(BackgroundImage))
        {
            classes.Add("osirion-hero-with-image");
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the background style for the hero section
    /// </summary>
    private string GetBackgroundStyle()
    {
        var styles = new List<string>();

        if (!string.IsNullOrEmpty(BackgroundImage))
        {
            styles.Add($"background-image: url('{BackgroundImage}')");
        }

        if (!string.IsNullOrEmpty(BackgroundColor))
        {
            styles.Add($"background-color: {BackgroundColor}");
        }

        if (!string.IsNullOrEmpty(TextColor))
        {
            styles.Add($"color: {TextColor}");
        }

        styles.Add($"min-height: {MinHeight}");

        return string.Join("; ", styles);
    }
}