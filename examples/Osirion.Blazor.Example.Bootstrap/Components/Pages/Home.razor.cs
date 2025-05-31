using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Home(IContentProviderManager contentProviderManager) 
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
        var result = await contentProviderManager.GetContentByQueryAsync(new Cms.Domain.Repositories.ContentQuery { Locale = Locale, Slug = "introduction", ProviderId = "github-osirion" });
        Content = result?.FirstOrDefault();
    }
}
