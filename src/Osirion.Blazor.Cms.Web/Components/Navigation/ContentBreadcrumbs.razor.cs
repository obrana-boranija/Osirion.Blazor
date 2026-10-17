using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the ContentBreadcrumbs type.</summary>
public partial class ContentBreadcrumbs
{
    /// <summary>
    /// Gets or sets the item being shown. Its title is the last crumb, and its <c>Directory</c> is the deepest
    /// directory of the trail when <see cref="Directory"/> is not set (read once, when the component is first
    /// initialized).
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Gets or sets the deepest directory of the trail. The crumbs are its parents up to the root, without a locale
    /// folder such as <c>en</c>.
    /// </summary>
    [Parameter]
    public DirectoryItem? Directory { get; set; }

    /// <summary>Gets or sets whether the home link is rendered first.</summary>
    [Parameter]
    public bool ShowHome { get; set; } = true;

    /// <summary>Gets or sets the text of the home link.</summary>
    [Parameter]
    public string HomeText { get; set; } = "Home";

    /// <summary>Gets or sets the target of the home link. Defaults to <c>/</c>.</summary>
    [Parameter]
    public string HomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets whether the item's title, the last crumb, is left out, for example when the page heading
    /// already shows it.
    /// </summary>
    [Parameter]
    public bool HideCurrentItem { get; set; } = false;

    /// <summary>
    /// Gets or sets a function that builds each directory link. Without it, a link goes to the address the
    /// directory's <c>_index.md</c> declares in <c>permalink</c>, otherwise to the directory's path without the
    /// locale folder.
    /// </summary>
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
        return DirectoryUrlFormatter?.Invoke(directory) ?? ContentUrls.DirectoryUrl(directory);
    }

    /// <summary>
    /// The directories from the root to <see cref="Directory"/>, in that order, without the locale folder of localized
    /// content (<c>content/en</c>), which has no page.
    /// </summary>
    private List<DirectoryItem> DirectoryPath()
    {
        var path = new List<DirectoryItem>();
        for (var current = Directory; current is not null; current = current.Parent)
        {
            if (!ContentUrls.IsLocaleFolder(current))
            {
                path.Insert(0, current);
            }
        }

        return path;
    }
}
