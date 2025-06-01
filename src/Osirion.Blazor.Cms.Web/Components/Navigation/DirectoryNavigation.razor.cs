using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

public partial class DirectoryNavigation
{
    [Parameter]
    public IReadOnlyList<DirectoryItem>? Directories { get; set; }

    [Parameter]
    public string? CurrentDirectory { get; set; }

    [Parameter]
    public string? ExpandedDirectory { get; set; }

    [Parameter]
    public bool ExpandAllSubdirectories { get; set; } = false;

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public bool ShowItemCount { get; set; } = true;

    [Parameter]
    public bool ShowSubdirectories { get; set; } = true;

    [Parameter]
    public string LoadingText { get; set; } = "Loading navigation...";

    [Parameter]
    public string NoContentText { get; set; } = "No directories available.";

    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    [Parameter]
    public string? Title { get; set; }

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