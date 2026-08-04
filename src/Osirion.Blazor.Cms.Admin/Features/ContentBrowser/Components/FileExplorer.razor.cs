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

    private async Task CreateNewFile()
    {
        // Publish event to create new file in current directory
        EventPublisher.Publish(new CreateNewContentEvent(ViewModel.CurrentPath));

        if (OnCreateFile.HasDelegate)
        {
            await OnCreateFile.InvokeAsync();
        }
    }
}
