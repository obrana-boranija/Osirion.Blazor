using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using System.Globalization;

namespace Osirion.Blazor.Example.Components.Layout;

public partial class NavMenu(IContentProviderManager contentProviderManager)
{
    private IReadOnlyList<DirectoryItem> _directories = [];

    protected override async Task OnInitializedAsync()
    {
        var locale = CultureInfo.CurrentUICulture;

        _directories = await contentProviderManager.GetDirectoryTreeAsync(locale.Name);

        await base.OnInitializedAsync();
    }
}
