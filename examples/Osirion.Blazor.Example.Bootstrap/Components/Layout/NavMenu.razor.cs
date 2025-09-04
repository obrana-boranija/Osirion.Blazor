using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using System.Globalization;

namespace Osirion.Blazor.Example.Bootstrap.Components.Layout;

public partial class NavMenu(IContentProviderManager contentProviderManager)
{
    private IReadOnlyList<DirectoryItem> _directories = [];

    protected override async Task OnInitializedAsync()
    {
        var locale = CultureInfo.CurrentUICulture;

        // Get the full directory tree for the locale
        var allDirectories = await contentProviderManager.GetDirectoryTreeAsync("en"/*locale.Name*/);

        // Filter to only root directories (directories with no parent) 
        // because we only want top-level directories in the navigation
        _directories = allDirectories.Where(d => d.Parent?.Parent == null).ToList();

        await base.OnInitializedAsync();
    }
}