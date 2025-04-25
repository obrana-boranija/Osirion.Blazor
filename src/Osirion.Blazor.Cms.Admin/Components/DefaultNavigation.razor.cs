using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components;

public partial class DefaultNavigation(NavigationManager navigationManager)
{
    [Parameter]
    public bool Expanded { get; set; } = true;

    private string GetActiveClass(string path)
    {
        var currentPath = navigationManager.Uri.ToLower();
        return currentPath.Contains(path.ToLower()) ? "active" : "";
    }
}
