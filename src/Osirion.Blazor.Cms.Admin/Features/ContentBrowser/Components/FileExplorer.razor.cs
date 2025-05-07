using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;

public partial class FileExplorer : BaseComponent
{
    [Parameter]
    public EventCallback OnFileSelected { get; set; }

    [Parameter]
    public EventCallback OnCreateFile { get; set; }

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