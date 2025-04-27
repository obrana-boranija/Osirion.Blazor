using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components;

public partial class AuthGuard(IAuthenticationService authService, NavigationManager navigationManager)
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string LoginPath { get; set; } = "/admin/login";

    [Parameter]
    public bool ShowLoginForm { get; set; } = false;

    [Parameter]
    public EventCallback<bool> OnAuthResult { get; set; }

    private string ReturnUrl => navigationManager.ToBaseRelativePath(navigationManager.Uri);

    private void RedirectToLogin()
    {
        var returnUrl = Uri.EscapeDataString(ReturnUrl);
        navigationManager.NavigateTo($"{LoginPath}?returnUrl={returnUrl}");
    }

    private async Task HandleLoginResult(bool success)
    {
        if (OnAuthResult.HasDelegate)
        {
            await OnAuthResult.InvokeAsync(success);
        }
    }
}
