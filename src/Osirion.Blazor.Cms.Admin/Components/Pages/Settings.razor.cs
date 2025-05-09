using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;
public partial class Settings(IAuthenticationService AuthService)
{
    [Parameter]
    public string Theme { get; set; } = "light";

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    protected override void OnParametersSet()
    {
        // Set default theme
        if (string.IsNullOrEmpty(Theme))
        {
            Theme = "light";
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ThemeChanged.HasDelegate && !firstRender)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }
}
