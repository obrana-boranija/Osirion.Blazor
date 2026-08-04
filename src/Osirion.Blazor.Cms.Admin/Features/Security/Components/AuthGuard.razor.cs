using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Security.Components;

/// <summary>Defines the AuthGuard type.</summary>
public partial class AuthGuard
{
    [Inject]
    private IAuthenticationService authService { get; set; } = default!;

    [Inject]
    private NavigationManager navigationManager { get; set; } = default!;

    /// <summary>Gets or sets the ChildContent value.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the LoginPath value.</summary>
    [Parameter]
    public string LoginPath { get; set; } = "/osirion/login";

    /// <summary>Gets or sets the ShowLoginForm value.</summary>
    [Parameter]
    public bool ShowLoginForm { get; set; } = false;

    /// <summary>Gets or sets the OnAuthResult value.</summary>
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
