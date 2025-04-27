using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Components;

public partial class ContentBreadcrumbs
{
    [Parameter]
    public ContentItem? Content { get; set; }

    [Parameter]
    public DirectoryItem? Directory { get; set; }

    [Parameter]
    public bool ShowHome { get; set; } = true;

    [Parameter]
    public string HomeText { get; set; } = "Home";

    [Parameter]
    public string HomeUrl { get; set; } = "/";

    [Parameter]
    public bool HideCurrentItem { get; set; } = false;

    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    protected override void OnInitialized()
    {
        if (Directory == null && Content?.Directory != null)
        {
            Directory = Content.Directory;
        }
    }

    private string GetBreadcrumbsClass()
    {
        return $"osirion-breadcrumbs {CssClass}".Trim();
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

        while (current != null)
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
