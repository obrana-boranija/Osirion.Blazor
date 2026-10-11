using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.Navigation.Components;

/// <summary>Defines the DefaultNavigation type.</summary>
public partial class DefaultNavigation : BaseComponent
{
    /// <summary>Gets or sets the Expanded value.</summary>
    [Parameter]
    public bool Expanded { get; set; } = true;

    // These match the @page routes of the admin pages; a link to anything else would render a not-found page.
    private const string OverviewRoute = "/osirion";
    private const string DashboardRoute = "/osirion/dashboard";
    private const string ContentRoute = "/osirion/content";
    private const string EditContentRoute = "/osirion/content/edit";
    private const string SettingsRoute = "/osirion/settings";

    // An exact match keeps "/osirion" from lighting up on every admin page, as a prefix match would.
    private string GetActiveClass(params string[] routes)
    {
        var currentRoute = GetCurrentRoute();
        return routes.Any(route => string.Equals(route, currentRoute, StringComparison.OrdinalIgnoreCase)) ? "active" : "";
    }

    private string GetCurrentRoute()
    {
        var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        var queryStart = relativePath.IndexOfAny(['?', '#']);
        if (queryStart >= 0)
        {
            relativePath = relativePath[..queryStart];
        }

        return "/" + relativePath.TrimEnd('/');
    }
}
