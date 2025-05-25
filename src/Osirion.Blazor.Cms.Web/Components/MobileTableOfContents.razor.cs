using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// MobileTableOfContents component for displaying a collapsible table of contents on smaller screens
/// </summary>
public partial class MobileTableOfContents
{
    /// <summary>
    /// Gets or sets the HTML content to extract headings from
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the component title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Table of Contents";

    /// <summary>
    /// Gets or sets the minimum heading level to include
    /// </summary>
    [Parameter]
    public int MinLevel { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum heading level to include
    /// </summary>
    [Parameter]
    public int MaxLevel { get; set; } = 4;

    /// <summary>
    /// Gets or sets the maximum nesting depth
    /// </summary>
    [Parameter]
    public int MaxDepth { get; set; } = 2;

    /// <summary>
    /// Gets or sets the text to display when no headings are found
    /// </summary>
    [Parameter]
    public string EmptyText { get; set; } = "No headings found.";

    /// <summary>
    /// Gets or sets whether the component is expanded
    /// </summary>
    [Parameter]
    public bool IsExpanded { get; set; } = false;

    /// <summary>
    /// Event callback for when the expanded state changes
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsExpandedChanged { get; set; }

    /// <summary>
    /// Gets the CSS class for the mobile table of contents
    /// </summary>
    private string GetMobileTocClass()
    {
        return $"osirion-mobile-toc {CssClass}".Trim();
    }

    /// <summary>
    /// Toggles the expanded state of the component
    /// </summary>
    private async Task ToggleToc()
    {
        IsExpanded = !IsExpanded;

        if (IsExpandedChanged.HasDelegate)
        {
            await IsExpandedChanged.InvokeAsync(IsExpanded);
        }
    }
}