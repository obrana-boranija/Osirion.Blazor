using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Read more link component that provides a consistent "read more" or call-to-action link with optional icon.
/// This component is framework-agnostic and supports various customization options.
/// </summary>
public partial class OsirionReadMoreLink
{
    /// <summary>
    /// Gets or sets the link URL
    /// </summary>
    [Parameter, EditorRequired]
    public string Href { get; set; } = "#";

    /// <summary>
    /// Gets or sets the link text
    /// </summary>
    [Parameter]
    public string Text { get; set; } = "Read more";

    /// <summary>
    /// Gets or sets the aria-label for accessibility
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets whether to show the text
    /// </summary>
    [Parameter]
    public bool ShowText { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the icon
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Gets or sets the icon position relative to text
    /// </summary>
    [Parameter]
    public IconPosition IconPosition { get; set; } = IconPosition.Right;

    /// <summary>
    /// Gets or sets custom icon content. When provided, this overrides the default icon.
    /// </summary>
    [Parameter]
    public RenderFragment? IconContent { get; set; }

    /// <summary>
    /// Gets or sets the link variant for different styling
    /// </summary>
    [Parameter]
    public ReadMoreVariant Variant { get; set; } = ReadMoreVariant.Default;

    /// <summary>
    /// Gets or sets the link size
    /// </summary>
    [Parameter]
    public LinkSize Size { get; set; } = LinkSize.Normal;

    /// <summary>
    /// Gets or sets whether the link should be stretched (useful in cards)
    /// </summary>
    [Parameter]
    public bool Stretched { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the link should be displayed as a block element
    /// </summary>
    [Parameter]
    public bool Block { get; set; } = false;

    /// <summary>
    /// Gets or sets the link target (_blank, _self, etc.)
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets whether the link should have hover animation
    /// </summary>
    [Parameter]
    public bool Animated { get; set; } = true;

    /// <summary>
    /// Gets or sets additional child content to be rendered inside the link
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the CSS class for the link element
    /// </summary>
    private string GetLinkClass()
    {
        var classes = new List<string> { "osirion-read-more" };

        // Add framework-specific base classes
        classes.Add(GetFrameworkLinkClass());

        // Add variant class
        classes.Add($"osirion-read-more-{Variant.ToString().ToLower()}");

        // Add size class
        classes.Add($"osirion-read-more-{Size.ToString().ToLower()}");

        // Add layout classes
        if (Stretched)
        {
            classes.Add("osirion-read-more-stretched");
            classes.Add(GetFrameworkStretchedClass());
        }

        if (Block)
        {
            classes.Add("osirion-read-more-block");
        }

        if (Animated)
        {
            classes.Add("osirion-read-more-animated");
        }

        // Add icon position class
        if (ShowIcon)
        {
            classes.Add($"osirion-read-more-icon-{IconPosition.ToString().ToLower()}");
        }

        // Add custom class if provided
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets framework-specific link classes
    /// </summary>
    private string GetFrameworkLinkClass()
    {
        return Framework switch
        {
            CssFramework.Bootstrap => Stretched ? "" : "icon-link icon-link-hover", // Don't add icon-link when stretched
            CssFramework.FluentUI => "osirion-fluent-link",
            CssFramework.MudBlazor => "mud-link",
            CssFramework.Radzen => "rz-link",
            _ => "osirion-link"
        };
    }

    /// <summary>
    /// Gets framework-specific stretched link classes
    /// </summary>
    private string GetFrameworkStretchedClass()
    {
        return Framework switch
        {
            CssFramework.Bootstrap => "stretched-link",
            CssFramework.FluentUI => "osirion-fluent-stretched",
            CssFramework.MudBlazor => "mud-stretched",
            CssFramework.Radzen => "rz-stretched",
            _ => "osirion-stretched"
        };
    }

    /// <summary>
    /// Gets the inline style for the link element
    /// </summary>
    private string GetLinkStyle()
    {
        var styles = new List<string>();

        if (!string.IsNullOrWhiteSpace(Style))
        {
            styles.Add(Style);
        }

        return string.Join("; ", styles) + (styles.Count > 0 ? ";" : "");
    }

    /// <summary>
    /// Gets the effective aria-label
    /// </summary>
    private string GetEffectiveAriaLabel()
    {
        return AriaLabel ?? Text ?? "Read more";
    }

    /// <summary>
    /// Renders the default icon based on variant and framework
    /// </summary>
    private RenderFragment GetDefaultIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "16");
        builder.AddAttribute(3, "height", "16");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "viewBox", "0 0 16 16");
        builder.AddAttribute(6, "aria-hidden", "true");

        switch (Variant)
        {
            case ReadMoreVariant.Arrow:
                // Arrow right icon
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "fill-rule", "evenodd");
                builder.AddAttribute(9, "d", "M1 8a.5.5 0 0 1 .5-.5h11.793l-3.147-3.146a.5.5 0 0 1 .708-.708l4 4a.5.5 0 0 1 0 .708l-4 4a.5.5 0 0 1-.708-.708L13.293 8.5H1.5A.5.5 0 0 1 1 8");
                builder.CloseElement();
                break;

            case ReadMoreVariant.External:
                // External link icon
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "fill-rule", "evenodd");
                builder.AddAttribute(9, "d", "M8.636 3.5a.5.5 0 0 0-.5-.5H1.5A1.5 1.5 0 0 0 0 4.5v10A1.5 1.5 0 0 0 1.5 16h10a1.5 1.5 0 0 0 1.5-1.5V7.864a.5.5 0 0 0-1 0V14.5a.5.5 0 0 1-.5.5h-10a.5.5 0 0 1-.5-.5v-10a.5.5 0 0 1 .5-.5h6.636a.5.5 0 0 0 .5-.5");
                builder.CloseElement();
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "fill-rule", "evenodd");
                builder.AddAttribute(9, "d", "M16 .5a.5.5 0 0 0-.5-.5h-5a.5.5 0 0 0 0 1h3.793L6.146 9.146a.5.5 0 1 0 .708.708L15 1.707V5.5a.5.5 0 0 0 1 0z");
                builder.CloseElement();
                break;

            case ReadMoreVariant.Download:
                // Download icon
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "d", "M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5");
                builder.CloseElement();
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "d", "M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708z");
                builder.CloseElement();
                break;

            default:
                // Default chevron right icon
                builder.OpenElement(7, "path");
                builder.AddAttribute(8, "fill-rule", "evenodd");
                builder.AddAttribute(9, "d", "M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708");
                builder.CloseElement();
                break;
        }

        builder.CloseElement();
    };

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Set aria-label if not provided
        if (string.IsNullOrWhiteSpace(AriaLabel))
        {
            AriaLabel = GetEffectiveAriaLabel();
        }
    }
}

/// <summary>
/// Read more link variants
/// </summary>
public enum ReadMoreVariant
{
    /// <summary>
    /// Default chevron right style
    /// </summary>
    Default,

    /// <summary>
    /// Arrow right style
    /// </summary>
    Arrow,

    /// <summary>
    /// External link style
    /// </summary>
    External,

    /// <summary>
    /// Download style
    /// </summary>
    Download,

    /// <summary>
    /// Plain text without icon
    /// </summary>
    Plain,

    /// <summary>
    /// Button-like appearance
    /// </summary>
    Button
}

/// <summary>
/// Icon position relative to text
/// </summary>
public enum IconPosition
{
    /// <summary>
    /// Icon positioned to the left of text
    /// </summary>
    Left,

    /// <summary>
    /// Icon positioned to the right of text
    /// </summary>
    Right
}

/// <summary>
/// Link size options
/// </summary>
public enum LinkSize
{
    /// <summary>
    /// Small size
    /// </summary>
    Small,

    /// <summary>
    /// Normal size (default)
    /// </summary>
    Normal,

    /// <summary>
    /// Large size
    /// </summary>
    Large
}