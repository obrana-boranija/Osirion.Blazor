using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Components;

/// <summary>
/// A GitHub login form with a personal access token field and, when configured, GitHub OAuth.
/// </summary>
/// <remarks>
/// It never signs anyone in with the personal access token configured for the server; it only shows that one is
/// configured. Browser storage and the OAuth code exchange are used only after the first interactive render, never
/// while prerendering. Place it only on pages that require the admin authorization policy.
/// </remarks>
public partial class Login
{
    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = "Osirion CMS Admin";

    /// <summary>Gets or sets the Description value.</summary>
    [Parameter]
    public string Description { get; set; } = "Sign in to manage your content.";

    /// <summary>Gets or sets the EnableGitHubAuth value. It is replaced by whether GitHub OAuth is configured.</summary>
    [Parameter]
    public bool EnableGitHubAuth { get; set; } = false;

    /// <summary>Gets or sets the local path to open after login. Any other value is replaced by <c>/osirion</c>.</summary>
    [Parameter]
    public string ReturnUrl { get; set; } = "/admin";

    /// <summary>Gets or sets the OnLoginResult value.</summary>
    [Parameter]
    public EventCallback<bool> OnLoginResult { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ViewModel.StateChanged += StateHasChanged;
        ViewModel.ReturnUrl = LocalUrl.GetSafeLocalPath(ReturnUrl, CmsConstants.Routes.Dashboard);

        // Check if GitHub OAuth is configured
        EnableGitHubAuth = ViewModel.IsGitHubOAuthConfigured;
    }

    /// <summary>Completes a GitHub OAuth login or restores a stored login after the first interactive render.</summary>
    /// <param name="firstRender">Whether this is the first render of the component</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Never runs while prerendering, so browser storage and the single-use OAuth state are touched only once
        if (!firstRender)
        {
            return;
        }

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        if (queryParameters.TryGetValue("code", out string code))
        {
            queryParameters.TryGetValue("state", out string state);
            await LoginWithGitHubCodeAsync(code, state);
            return;
        }

        await ExecuteAsync(ViewModel.InitializeAsync);
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private Task LoginWithGitHub()
    {
        return ExecuteAsync(ViewModel.StartGitHubLoginAsync);
    }

    private async Task LoginWithGitHubCodeAsync(string code, string state)
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoginWithGitHubAsync(code, state);

            if (OnLoginResult.HasDelegate)
            {
                await OnLoginResult.InvokeAsync(string.IsNullOrWhiteSpace(ViewModel.ErrorMessage));
            }
        });
    }

    private async Task LoginWithToken()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoginWithTokenAsync();

            if (OnLoginResult.HasDelegate)
            {
                await OnLoginResult.InvokeAsync(string.IsNullOrWhiteSpace(ViewModel.ErrorMessage));
            }
        });
    }
}
