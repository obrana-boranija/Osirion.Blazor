using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;
public partial class LoginPage
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string ReturnUrl { get; set; } = "/osirion";

    [Parameter]
    public string Theme { get; set; } = "light";

    private string GetReturnUrl()
    {
        // Sanitize and validate return URL to prevent open redirect
        if (string.IsNullOrEmpty(ReturnUrl))
        {
            return "/osirion";
        }

        // Only accept relative URLs
        if (ReturnUrl.StartsWith("/"))
        {
            return ReturnUrl;
        }

        return "/osirion";
    }

    private void HandleLoginResult(bool success)
    {
        if (success)
        {
            NavigationManager.NavigateTo(GetReturnUrl());
        }
    }

    private string GetLoginPageClass()
    {
        return $"osirion-admin-login-page osirion-admin-theme-{Theme} {CssClass}".Trim();
    }
}
