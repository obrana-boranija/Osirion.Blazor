using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays navigable content directories.</summary>
public partial class DirectoryNavigation
{
    /// <summary>
    /// Gets or sets the directories to list. Nothing is loaded: when <c>null</c> or empty, <see
    /// cref="NoContentText"/> is shown.
    /// </summary>
    [Parameter]
    public IReadOnlyList<DirectoryItem>? Directories { get; set; }

    /// <summary>
    /// Gets or sets the path of the current directory, compared with the directory's <c>Path</c>. Its link gets the
    /// <c>osirion-active</c> class.
    /// </summary>
    [Parameter]
    public string? CurrentDirectory { get; set; }

    /// <summary>
    /// Gets or sets the path of the top-level directory whose subdirectories are shown, compared with the
    /// directory's <c>Path</c>.
    /// </summary>
    [Parameter]
    public string? ExpandedDirectory { get; set; }

    /// <summary>Gets or sets whether the subdirectories of every top-level directory are shown.</summary>
    [Parameter]
    public bool ExpandAllSubdirectories { get; set; } = false;

    /// <summary>
    /// Gets or sets whether <see cref="LoadingText"/> is shown instead of the list, for a page that loads the
    /// directories itself.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets whether a count is shown after each name. The count is the number of subdirectories, not of
    /// content items.
    /// </summary>
    [Parameter]
    public bool ShowItemCount { get; set; } = true;

    /// <summary>
    /// Gets or sets whether subdirectories can be shown at all. One level is rendered, and only for an expanded
    /// directory.
    /// </summary>
    [Parameter]
    public bool ShowSubdirectories { get; set; } = true;

    /// <summary>Gets or sets the text shown while <see cref="IsLoading"/> is <c>true</c>.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading navigation...";

    /// <summary>Gets or sets the text shown when <see cref="Directories"/> is <c>null</c> or empty.</summary>
    [Parameter]
    public string NoContentText { get; set; } = "No directories available.";

    /// <summary>
    /// Gets or sets a function that builds each link. Without it, a link goes to the address the directory's
    /// <c>_index.md</c> declares in <c>permalink</c>, otherwise to the directory's path without the locale folder.
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>Gets or sets the heading above the list, rendered as an <c>h3</c>. No heading when <c>null</c>.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the callback run with the directory when its link is clicked in an interactive render mode.
    /// When it is set, the click no longer navigates. Under static SSR it is not called.
    /// </summary>
    [Parameter]
    public EventCallback<DirectoryItem> DirectoryClicked { get; set; }

    private string GetDirectoryNavClass()
    {
        return $"osirion-directory-navigation".Trim();
    }

    private string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? ContentUrls.DirectoryUrl(directory);
    }

    private string GetLinkClass(DirectoryItem directory)
    {
        var isActive = directory.Path == CurrentDirectory;
        return $"osirion-directory-link {(isActive ? "osirion-active" : "")}".Trim();
    }

    private async Task OnDirectoryClick(DirectoryItem directory)
    {
        if (DirectoryClicked.HasDelegate)
        {
            await DirectoryClicked.InvokeAsync(directory);
        }
    }

    //public class DirectoryInfo
    //{
    //    public string Path { get; set; } = string.Empty;
    //    public string Name { get; set; } = string.Empty;
    //    public int ItemCount { get; set; }
    //    public IReadOnlyList<DirectoryInfo>? Subdirectories { get; set; }
    //}
}
