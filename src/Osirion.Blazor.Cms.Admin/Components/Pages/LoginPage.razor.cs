using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

/// <summary>
/// The admin page where a user signs in to GitHub with a personal access token or GitHub OAuth.
/// </summary>
/// <remarks>
/// The page sits behind the admin authorization policy like every other admin page. It never signs anyone in with
/// the personal access token configured for the server; it only shows that one is configured. Browser storage and
/// the OAuth code exchange are used only after the first interactive render, never while prerendering.
/// </remarks>
public partial class LoginPage
{
    [Inject]
    private LoginViewModel ViewModel { get; set; } = null!;

    /// <summary>Gets or sets the local path to open after login. Any other value is replaced by <c>/osirion</c>.</summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string ReturnUrl { get; set; } = "/osirion";

    private string AccessToken
    {
        get => ViewModel.AccessToken;
        set => ViewModel.AccessToken = value;
    }

    private bool IsLoggingIn => ViewModel.IsLoggingIn;
    private bool IsShowingTokenInput => ViewModel.IsShowingTokenInput;
    private bool EnableGitHubAuth => ViewModel.IsGitHubOAuthConfigured;
    private bool IsServerCredentialConfigured => ViewModel.IsServerCredentialConfigured;
    private string? DisplayedErrorMessage => ViewModel.ErrorMessage ?? ErrorMessage;

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ViewModel.StateChanged += StateHasChanged;
        ViewModel.ReturnUrl = LocalUrl.GetSafeLocalPath(ReturnUrl, CmsConstants.Routes.Dashboard);
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
            await ExecuteAsync(() => ViewModel.LoginWithGitHubAsync(code, state));
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

    private Task LoginWithToken()
    {
        return ExecuteAsync(ViewModel.LoginWithTokenAsync);
    }

    private void ToggleTokenInput()
    {
        ViewModel.ToggleTokenInput();
    }
}
