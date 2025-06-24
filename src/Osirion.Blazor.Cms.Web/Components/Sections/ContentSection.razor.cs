using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// ContentSection component for displaying sections of content with optional titles and actions
/// </summary>
public partial class ContentSection
{
    /// <summary>
    /// Gets or sets the section title
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the section subtitle
    /// </summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets the HTML content to display
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the section variant: "default", "highlight", "bordered", "background"
    /// </summary>
    [Parameter]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Gets or sets the section width: "narrow", "normal", "wide", "full"
    /// </summary>
    [Parameter]
    public string Width { get; set; } = "normal";

    /// <summary>
    /// Gets or sets the text alignment: "left", "center", "right"
    /// </summary>
    [Parameter]
    public string TextAlign { get; set; } = "left";

    /// <summary>
    /// Gets or sets the action button text
    /// </summary>
    [Parameter]
    public string? ActionText { get; set; }

    /// <summary>
    /// Gets or sets the action button URL
    /// </summary>
    [Parameter]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets the padding size: "none", "small", "normal", "large"
    /// </summary>
    [Parameter]
    public string Padding { get; set; } = "normal";

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the CSS class for the section
    /// </summary>
    private string GetSectionClass()
    {
        var classes = new List<string> { "osirion-content-section" };

        classes.Add($"osirion-section-variant-{Variant}");
        classes.Add($"osirion-section-width-{Width}");
        classes.Add($"osirion-section-align-{TextAlign}");
        classes.Add($"osirion-section-padding-{Padding}");

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}