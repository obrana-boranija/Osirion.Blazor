using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Home(IContentProviderManager ContentProviderManager) 
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    private ContentItem? Content { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadContentAsync();
    }

    private async Task LoadContentAsync()
    {
        var provider = ContentProviderManager.GetDefaultProvider();
        if (provider == null)
            return;

        var result = await provider.GetItemsByQueryAsync(new Cms.Domain.Repositories.ContentQuery { Locale = Locale, Url = "blog/analytics-integration-in-blazor" });
        Content = result?.FirstOrDefault();
    }
}
