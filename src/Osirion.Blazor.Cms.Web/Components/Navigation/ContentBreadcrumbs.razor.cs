using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the ContentBreadcrumbs type.</summary>
public partial class ContentBreadcrumbs
{
    /// <summary>Performs the Content operation.</summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>Performs the Directory operation.</summary>
    [Parameter]
    public DirectoryItem? Directory { get; set; }

    /// <summary>Gets or sets the ShowHome value.</summary>
    [Parameter]
    public bool ShowHome { get; set; } = true;

    /// <summary>Gets or sets the HomeText value.</summary>
    [Parameter]
    public string HomeText { get; set; } = "Home";

    /// <summary>Gets or sets the HomeUrl value.</summary>
    [Parameter]
    public string HomeUrl { get; set; } = "/";

    /// <summary>Gets or sets the HideCurrentItem value.</summary>
    [Parameter]
    public bool HideCurrentItem { get; set; } = false;

    /// <summary>Gets or sets the DirectoryUrlFormatter value.</summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        if (Directory is null && Content?.Directory is not null)
        {
            Directory = Content.Directory;
        }
    }

    private string GetBreadcrumbsClass()
    {
        return $"osirion-breadcrumbs {Class}".Trim();
    }

    private string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? $"/{directory.Path}";
    }

    private RenderFragment RenderDirectoryPath(DirectoryItem directory) => builder =>
    {
        // Build the directory path from root to current
        var path = new List<DirectoryItem>();
        var current = directory;

        while (current is not null)
        {
            path.Add(current);
            current = current.Parent;
        }

        // Render directories in reverse order (root to leaf)
        foreach (var dir in path.AsEnumerable().Reverse())
        {
            builder.OpenElement(0, "li");
            builder.AddAttribute(1, "class", "osirion-breadcrumbs-item");

            builder.OpenElement(2, "a");
            builder.AddAttribute(3, "href", GetDirectoryUrl(dir));
            builder.AddAttribute(4, "class", "osirion-breadcrumbs-link");
            builder.AddContent(5, dir.Name);
            builder.CloseElement(); // a

            builder.CloseElement(); // li
        }
    };
}
