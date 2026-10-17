using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Examples.Shared.Docs;

namespace Osirion.Blazor.Examples.Shared.Pages;

/// <summary>The component gallery at /components.</summary>
public partial class ComponentGallery(IContentProviderManager contentProviderManager, NavigationManager navigationManager)
{
    private DocsOutline? _outline;
    private ContentItem? _page;

    // Groups shown in the gallery that have something to show: components, or a link to where they are documented.
    private IEnumerable<DocsSection> GallerySections => _outline?.Sections
        .Where(section => section.Group.InGallery && (section.Members.Count > 0 || section.Group.SeeAlso is not null))
        ?? [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        var provider = contentProviderManager.GetDefaultProvider();
        var items = provider is null ? [] : await provider.GetAllItemsAsync();
        _outline = DocsOutline.Build(items);
        _page = _outline.Find(DocsOutline.GalleryUrl);

        if (_page is null)
        {
            navigationManager.NotFound();
        }
    }
}
