using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Examples.Shared.Docs;

/// <summary>Renders the documentation page at an address, or answers 404 when there is none.</summary>
public partial class DocsArticle(IContentProviderManager contentProviderManager, NavigationManager navigationManager)
{
    private const string SourceUrl = "https://github.com/demonjic/Osirion.Blazor/edit/master/content/";

    private DocsOutline? _outline;
    private ContentItem? _page;
    private ContentItem? _previous;
    private ContentItem? _next;

    /// <summary>The page's address, such as <c>/components/tabs</c>.</summary>
    [Parameter, EditorRequired]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        var provider = contentProviderManager.GetDefaultProvider();
        var items = provider is null ? [] : await provider.GetAllItemsAsync();
        _outline = DocsOutline.Build(items);
        _page = _outline.Find(Url);
        (_previous, _next) = _page is null ? (null, null) : _outline.NeighboursOf(_page);

        if (_page is null)
        {
            navigationManager.NotFound();
        }
    }

    // The page's Markdown file on GitHub, opened in the editor.
    private static string EditUrl(ContentItem item) => SourceUrl + item.Path.TrimStart('/');
}
