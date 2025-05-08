namespace Osirion.Blazor.Cms.Admin.Features.Layouts.Models;

/// <summary>
/// Represents a breadcrumb navigation item
/// </summary>
public class BreadcrumbItem
{
    /// <summary>
    /// Gets or sets the displayed text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the navigation URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Creates a new breadcrumb item
    /// </summary>
    public BreadcrumbItem(string text, string url = "")
    {
        Text = text;
        Url = url;
    }
}