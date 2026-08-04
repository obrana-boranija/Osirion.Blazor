using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays navigable content directories.</summary>
public partial class DirectoryNavigation
{
    /// <summary>Gets or sets the directories to display.</summary>
    [Parameter]
    public IReadOnlyList<DirectoryItem>? Directories { get; set; }

    /// <summary>Gets or sets the current directory path.</summary>
    [Parameter]
    public string? CurrentDirectory { get; set; }

    /// <summary>Gets or sets the directory path that is expanded.</summary>
    [Parameter]
    public string? ExpandedDirectory { get; set; }

    /// <summary>Gets or sets a value indicating whether all subdirectories are expanded.</summary>
    [Parameter]
    public bool ExpandAllSubdirectories { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether the component is loading.</summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>Gets or sets a value indicating whether item counts are shown.</summary>
    [Parameter]
    public bool ShowItemCount { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether subdirectories are shown.</summary>
    [Parameter]
    public bool ShowSubdirectories { get; set; } = true;

    /// <summary>Gets or sets the loading message.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading navigation...";

    /// <summary>Gets or sets the empty-state message.</summary>
    [Parameter]
    public string NoContentText { get; set; } = "No directories available.";

    /// <summary>Gets or sets a formatter for directory URLs.</summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>Gets or sets the navigation title.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets the callback invoked when a directory is clicked.</summary>
    [Parameter]
    public EventCallback<DirectoryItem> DirectoryClicked { get; set; }

    private string GetDirectoryNavClass()
    {
        return $"osirion-directory-navigation".Trim();
    }

    private string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? $"/{directory.Url}";
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
