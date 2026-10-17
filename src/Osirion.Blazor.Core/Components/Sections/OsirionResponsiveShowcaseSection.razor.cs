using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Core.Components.Sections;

/// <summary>
/// A responsive showcase component that simulates different viewport sizes for component demonstrations.
/// This component is SSR-compatible and works without JavaScript dependencies for basic functionality.
/// </summary>
public partial class OsirionResponsiveShowcaseSection : ComponentBase
{
    /// <summary>
    /// The heading above the frame, rendered as an <c>h3</c>.
    /// </summary>
    [Parameter] public string Title { get; set; } = "Component Preview";

    /// <summary>
    /// The content to preview inside the frame; without it a placeholder is shown.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the current width is described under the frame.
    /// </summary>
    [Parameter] public bool ShowDimensions { get; set; } = true;

    /// <summary>
    /// Whether a Show Code button appears; it needs <see cref="CodeSnippet"/>.
    /// </summary>
    [Parameter] public bool ShowCode { get; set; } = true;

    /// <summary>
    /// The code shown under the preview when the button is pressed, rendered as encoded text.
    /// </summary>
    [Parameter] public string? CodeSnippet { get; set; }

    /// <summary>
    /// The width the frame starts in: <c>Desktop</c> (1200px), <c>Tablet</c> (768px) or <c>Mobile</c> (375px), each
    /// capped at the available width.
    /// </summary>
    [Parameter] public ViewportMode InitialViewport { get; set; } = ViewportMode.Desktop;

    /// <summary>
    /// Whether the frame shows the browser bar (dots and address) above the content; <see langword="false"/> leaves it
    /// out and the frame loses the <c>with-chrome</c> class.
    /// </summary>
    [Parameter] public bool ShowBrowserChrome { get; set; } = true;

    /// <summary>
    /// Extra CSS classes on the component's root element.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// An inline style for the frame, added after its width declarations.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Unmatched attributes, written to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the current viewport mode.
    /// </summary>
    public ViewportMode CurrentViewport { get; private set; }

    /// <summary>
    /// Gets or sets whether the code content is currently shown.
    /// </summary>
    private bool ShowCodeContent { get; set; } = false;

    /// <summary>
    /// Enumeration of supported viewport modes for responsive testing.
    /// </summary>
    public enum ViewportMode
    {
        /// <summary>Represents a desktop viewport.</summary>
        Desktop,
        /// <summary>Represents a tablet viewport.</summary>
        Tablet,
        /// <summary>Represents a mobile viewport.</summary>
        Mobile
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        CurrentViewport = InitialViewport;
    }

    /// <summary>
    /// Sets the current viewport mode and triggers a re-render.
    /// </summary>
    /// <param name="viewport">The viewport mode to set.</param>
    private void SetViewport(ViewportMode viewport)
    {
        if (CurrentViewport != viewport)
        {
            CurrentViewport = viewport;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Toggles the visibility of the code snippet.
    /// </summary>
    private void ToggleCode()
    {
        ShowCodeContent = !ShowCodeContent;
        StateHasChanged();
    }

    /// <summary>
    /// Gets the CSS class name for the current viewport.
    /// </summary>
    /// <returns>The viewport CSS class.</returns>
    private string GetViewportClass()
    {
        return CurrentViewport switch
        {
            ViewportMode.Desktop => "viewport-desktop",
            ViewportMode.Tablet => "viewport-tablet",
            ViewportMode.Mobile => "viewport-mobile",
            _ => "viewport-desktop"
        };
    }

    /// <summary>
    /// Gets the inline styles for the current viewport to simulate different screen sizes.
    /// </summary>
    /// <returns>The viewport CSS styles.</returns>
    private string GetViewportStyle()
    {
        var styles = new List<string>();

        switch (CurrentViewport)
        {
            case ViewportMode.Desktop:
                styles.Add("width: 1200px");
                styles.Add("max-width: 100%");
                styles.Add("min-height: 600px");
                break;

            case ViewportMode.Tablet:
                styles.Add("width: 768px");
                styles.Add("max-width: 100%");
                styles.Add("min-height: 500px");
                break;

            case ViewportMode.Mobile:
                styles.Add("width: 375px");
                styles.Add("max-width: 100%");
                styles.Add("min-height: 600px");
                break;
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            styles.Add(Style);
        }

        return string.Join("; ", styles);
    }

    /// <summary>
    /// Gets the title for the viewport chrome address bar.
    /// </summary>
    /// <returns>The viewport title.</returns>
    private string GetViewportTitle()
    {
        return CurrentViewport switch
        {
            ViewportMode.Desktop => "example.com - Desktop View",
            ViewportMode.Tablet => "example.com - Tablet View",
            ViewportMode.Mobile => "example.com - Mobile View",
            _ => "example.com"
        };
    }

    /// <summary>
    /// Gets the current viewport dimensions description.
    /// </summary>
    /// <returns>The dimensions description.</returns>
    private string GetCurrentDimensions()
    {
        return CurrentViewport switch
        {
            ViewportMode.Desktop => "Desktop: 1200px × 600px+ (Large screens, laptops)",
            ViewportMode.Tablet => "Tablet: 768px × 500px+ (iPad, tablets)",
            ViewportMode.Mobile => "Mobile: 375px × 600px+ (iPhone, Android phones)",
            _ => ""
        };
    }

    /// <summary>
    /// Gets the combined CSS classes for the component root element.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetCssClasses()
    {
        var classes = new List<string> { "osirion-responsive-showcase" };

        if (ShowBrowserChrome)
        {
            classes.Add("with-chrome");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}
