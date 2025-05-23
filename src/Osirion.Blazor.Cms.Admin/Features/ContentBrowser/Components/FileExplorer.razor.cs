using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;

public partial class FileExplorer : IDisposable
{
    [Parameter]
    public EventCallback<GitHubItem> OnFileSelected { get; set; }

    [Parameter]
    public EventCallback OnCreateFile { get; set; }

    [Parameter]
    public string Title { get; set; } = "Files";

    [Parameter]
    public bool CanCreateFile { get; set; } = true;

    [Parameter]
    public bool CanDeleteFile { get; set; } = true;

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadContentsAsync();
    }

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