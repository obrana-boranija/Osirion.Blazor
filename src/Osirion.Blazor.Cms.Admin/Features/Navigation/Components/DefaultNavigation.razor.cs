using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.Navigation.Components;

public partial class DefaultNavigation : BaseComponent
{
    [Parameter]
    public bool Expanded { get; set; } = true;

    private string GetActiveClass(string path)
    {
        var currentPath = NavigationManager.Uri.ToLower();
        return currentPath.Contains(path.ToLower()) ? "active" : "";
    }
}