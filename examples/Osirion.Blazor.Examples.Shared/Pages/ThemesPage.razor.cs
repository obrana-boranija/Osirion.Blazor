using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Examples.Shared.Docs;

namespace Osirion.Blazor.Examples.Shared.Pages;

/// <summary>The theme page at /themes.</summary>
public partial class ThemesPage(IContentProviderManager contentProviderManager, NavigationManager navigationManager)
{
    private ContentItem? _page;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        var provider = contentProviderManager.GetDefaultProvider();
        var items = provider is null ? [] : await provider.GetAllItemsAsync();
        _page = DocsOutline.Build(items).Find(DocsOutline.ThemesUrl);

        if (_page is null)
        {
            navigationManager.NotFound();
        }
    }
}
