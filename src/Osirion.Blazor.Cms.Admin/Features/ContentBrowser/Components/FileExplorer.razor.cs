using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;

/// <summary>Defines the FileExplorer type.</summary>
public partial class FileExplorer : IDisposable
{
    /// <summary>Gets or sets the OnFileSelected value.</summary>
    [Parameter]
    public EventCallback<GitHubItem> OnFileSelected { get; set; }

    /// <summary>Gets or sets the OnCreateFile value.</summary>
    [Parameter]
    public EventCallback OnCreateFile { get; set; }

    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = "Files";

    /// <summary>Gets or sets the CanCreateFile value.</summary>
    [Parameter]
    public bool CanCreateFile { get; set; } = true;

    /// <summary>Gets or sets the CanDeleteFile value.</summary>
    [Parameter]
    public bool CanDeleteFile { get; set; } = true;

    private string DisplayedPath => string.IsNullOrWhiteSpace(ViewModel.CurrentPath) ? "/" : ViewModel.CurrentPath;

    // The markup branches live here so the render tree keeps a low complexity.
    private string WhenSelected(GitHubItem item, string selectedClass, string otherClass) =>
        ViewModel.IsItemSelected(item) ? selectedClass : otherClass;

    private static string FileIconClass(GitHubItem item) =>
        item.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ? "bi-markdown-fill" : "bi-file-earmark";

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadContentsAsync();
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }
}
