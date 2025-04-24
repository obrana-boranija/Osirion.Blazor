using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Components;

public partial class DirectoryNavigation
{
    [Parameter]
    public IReadOnlyList<DirectoryInfo>? Directories { get; set; }

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
    public Func<DirectoryInfo, string>? DirectoryUrlFormatter { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public EventCallback<DirectoryInfo> DirectoryClicked { get; set; }

    private string GetDirectoryNavClass()
    {
        return $"osirion-directory-navigation {CssClass}".Trim();
    }

    private string GetDirectoryUrl(DirectoryInfo directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? $"/{directory.Path}";
    }

    private string GetLinkClass(DirectoryInfo directory)
    {
        var isActive = directory.Path == CurrentDirectory;
        return $"osirion-directory-link {(isActive ? "osirion-active" : "")}".Trim();
    }

    private async Task OnDirectoryClick(DirectoryInfo directory)
    {
        if (DirectoryClicked.HasDelegate)
        {
            await DirectoryClicked.InvokeAsync(directory);
        }
    }

    public class DirectoryInfo
    {
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public IReadOnlyList<DirectoryInfo>? Subdirectories { get; set; }
    }
}
