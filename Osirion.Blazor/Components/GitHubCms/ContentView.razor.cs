using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Models.Cms;
using Osirion.Blazor.Services.GitHub;

namespace Osirion.Blazor.Components.GitHubCms;

public partial class ContentView
{
    [Parameter]
    public string Path { get; set; } = string.Empty;

    [Inject]
    private IGitHubCmsService CmsService { get; set; } = default!;

    private bool IsLoading { get; set; } = true;
    private ContentItem? ContentItem { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadContentAsync();
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            ContentItem = await CmsService.GetContentItemByPathAsync(Path);
        }
        catch (Exception ex)
        {
            // Log error
            Console.Error.WriteLine($"Error loading content: {ex.Message}");
            ContentItem = null;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
