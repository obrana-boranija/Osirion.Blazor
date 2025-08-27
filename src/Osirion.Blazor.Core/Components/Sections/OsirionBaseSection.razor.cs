using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Base section component for creating structured content sections with optional headers, descriptions, and background styling.
/// This component provides consistent styling and layout for different CSS frameworks.
/// </summary>
public partial class OsirionBaseSection
{
    /// <summary>
    /// Gets or sets the section content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the section element
    /// </summary>
    [Parameter, EditorRequired]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the section title text
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets custom title content. When provided, this overrides the Title parameter.
    /// </summary>
    [Parameter]
    public RenderFragment? TitleContent { get; set; }

    /// <summary>
    /// Gets or sets the section description text
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets custom description content. When provided, this overrides the Description parameter.
    /// </summary>
    [Parameter]
    public RenderFragment? DescriptionContent { get; set; }

    /// <summary>
    /// Gets or sets the container CSS class. Different frameworks use different container classes.
    /// </summary>
    [Parameter]
    public string? ContainerClass { get; set; }

    /// <summary>
    /// Gets or sets the text alignment: Left, Center, Right, or Justify
    /// </summary>
    [Parameter]
    public Alignment TextAlignment { get; set; } = Alignment.Center;

    /// <summary>
    /// Gets or sets the background image URL
    /// </summary>
    [Parameter]
    public string? BackgroundImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the background color
    /// </summary>
    [Parameter]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text color override
    /// </summary>
    [Parameter]
    public string? TextColor { get; set; }

    /// <summary>
    /// Gets or sets whether to show an overlay when using background images for better text readability
    /// </summary>
    [Parameter]
    public bool ShowOverlay { get; set; } = true;

    /// <summary>
    /// Gets or sets the background pattern type
    /// </summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    /// <summary>
    /// Gets or sets whether to show the background pattern
    /// </summary>
    [Parameter]
    public bool ShowPattern { get; set; } = false;

    /// <summary>
    /// Gets or sets the padding size: Small, Medium, Large, or None
    /// </summary>
    [Parameter]
    public SectionPadding Padding { get; set; } = SectionPadding.Medium;

    /// <summary>
    /// Gets or sets whether the section has a divider/shadow effect
    /// </summary>
    [Parameter]
    public bool HasDivider { get; set; } = false;

    /// <summary>
    /// Gets the CSS class for the section element
    /// </summary>
    private string GetSectionClass()
    {
        var classes = new List<string> { "osirion-section" };

        // Add text alignment class
        classes.Add($"osirion-section-align-{TextAlignment.ToString().ToLower()}");

        // Add padding class
        classes.Add($"osirion-section-padding-{Padding.ToString().ToLower()}");

        // Add background styling classes
        if (!string.IsNullOrWhiteSpace(BackgroundImageUrl))
        {
            classes.Add("osirion-section-with-background");
        }

        if (ShowPattern && BackgroundPattern.HasValue)
        {
            classes.Add("osirion-section-with-pattern");
        }

        if (HasDivider)
        {
            classes.Add("osirion-section-divider");
        }

        // Add custom class if provided
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the CSS class for the container div
    /// </summary>
    private string GetContainerClass()
    {
        if (!string.IsNullOrWhiteSpace(ContainerClass))
        {
            return ContainerClass;
        }

        // Use framework-specific container classes
        return Framework switch
        {
            CssFramework.Bootstrap => "container-xxl",
            CssFramework.FluentUI => "osirion-container",
            CssFramework.MudBlazor => "mud-container",
            CssFramework.Radzen => "rz-container",
            _ => "osirion-container"
        };
    }

    /// <summary>
    /// Gets the CSS class for the header section
    /// </summary>
    private string GetHeaderClass()
    {
        var classes = new List<string> { "osirion-section-header" };

        // Add text alignment class for header
        classes.Add($"osirion-section-header-{TextAlignment.ToString().ToLower()}");

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the inline style for the section element
    /// </summary>
    private string GetSectionStyle()
    {
        var styles = new List<string>();

        if (!string.IsNullOrWhiteSpace(BackgroundImageUrl))
        {
            styles.Add($"background-image: url('{BackgroundImageUrl}')");
        }

        if (!string.IsNullOrWhiteSpace(BackgroundColor))
        {
            styles.Add($"background-color: {BackgroundColor}");
        }

        if (!string.IsNullOrWhiteSpace(TextColor))
        {
            styles.Add($"color: {TextColor}");
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            styles.Add(Style);
        }

        return string.Join("; ", styles) + (styles.Count > 0 ? ";" : "");
    }
}

/// <summary>
/// Enumeration for section padding sizes
/// </summary>
public enum SectionPadding
{
    /// <summary>
    /// No padding
    /// </summary>
    None,

    /// <summary>
    /// Small padding
    /// </summary>
    Small,

    /// <summary>
    /// Medium padding (default)
    /// </summary>
    Medium,

    /// <summary>
    /// Large padding
    /// </summary>
    Large
}