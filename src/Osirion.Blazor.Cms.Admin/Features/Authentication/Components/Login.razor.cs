using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Base;
using Osirion.Blazor.Cms.Admin.Common.Extensions;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Components;

public partial class Login(NavigationManager navigationManager) : LoadableComponentBase
{
    [Inject]
    private LoginViewModel ViewModel { get; set; } = default!;

    [Parameter]
    public string Title { get; set; } = "Osirion CMS Admin";

    [Parameter]
    public string Description { get; set; } = "Sign in to manage your content.";

    [Parameter]
    public string Theme { get; set; } = "light";

    [Parameter]
    public bool EnableGitHubAuth { get; set; } = false;

    [Parameter]
    public string ReturnUrl { get; set; } = "/admin";

    [Parameter]
    public EventCallback<bool> OnLoginResult { get; set; }

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
        ViewModel.ReturnUrl = ReturnUrl;
    }

    protected override async Task OnInitializedAsync()
    {
        // Check if we have a code parameter (from GitHub OAuth redirect)
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        var queryParameters = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (queryParameters["code"] is string code) // Access the "code" parameter safely
        {
            // Process GitHub OAuth login
            await ExecuteWithLoadingAsync(async () =>
            {
                await ViewModel.LoginWithGitHubAsync(code);

                if (OnLoginResult.HasDelegate)
                {
                    await OnLoginResult.InvokeAsync(!string.IsNullOrEmpty(ViewModel.ErrorMessage));
                }
            });
        }
    }

    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private void LoginWithGitHub()
    {
        // Redirect to GitHub OAuth authorize URL - in real implementation this would be configured
        ViewModel.ErrorMessage = "GitHub OAuth login requires additional configuration.";
    }

    private async Task LoginWithToken()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            await ViewModel.LoginWithTokenAsync();

            if (OnLoginResult.HasDelegate)
            {
                await OnLoginResult.InvokeAsync(!string.IsNullOrEmpty(ViewModel.ErrorMessage));
            }
        });
    }

    private string GetLoginClass()
    {
        return this.GetCssClassNames($"osirion-admin-theme-{Theme}");
    }
}