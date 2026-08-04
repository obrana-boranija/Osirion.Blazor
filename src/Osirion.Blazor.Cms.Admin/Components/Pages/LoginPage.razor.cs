using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Common;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

    /// <summary>Defines the public member type.</summary>
public partial class LoginPage
{
    [Inject]
    private LoginViewModel ViewModel { get; set; } = null!;

    [Inject]
    private IOptions<CmsAdminOptions> Options { get; set; } = null!;

    /// <summary>Gets or sets the ReturnUrl value.</summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string ReturnUrl { get; set; } = "/osirion";

    /// <summary>Gets or sets the Theme value.</summary>
    [Parameter]
    public new string Theme { get; set; } = "light";

    private string AccessToken
    {
        get => ViewModel.AccessToken;
        set => ViewModel.AccessToken = value;
    }

    private bool IsLoggingIn => ViewModel.IsLoggingIn;
    private bool IsShowingTokenInput => ViewModel.IsShowingTokenInput;

    private bool EnableGitHubAuth => !string.IsNullOrWhiteSpace(Options.Value.Authentication.GitHubClientId) &&
                                   !string.IsNullOrWhiteSpace(Options.Value.Authentication.GitHubClientSecret);

    private string GitHubAuthUrl => $"https://github.com/login/oauth/authorize?client_id={Options.Value.Authentication.GitHubClientId}&redirect_uri={Uri.EscapeDataString(GetRedirectUri())}&scope=repo";

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
        ViewModel.ReturnUrl = GetSanitizedReturnUrl();
    }

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        // Check if we have a configured PAT in options
        if (!string.IsNullOrWhiteSpace(Options.Value.Authentication.PersonalAccessToken))
        {
            // If we have a PAT in the options, try to use it directly
            ViewModel.AccessToken = Options.Value.Authentication.PersonalAccessToken;
            await LoginWithToken();
            return;
        }

        // Otherwise initialize normally
        await ViewModel.InitializeAsync();

        // Check if we have a code parameter (from GitHub OAuth redirect)
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        if (queryParameters.TryGetValue("code", out string code))
        {
            // Process GitHub OAuth login
            await LoginWithGitHubCodeAsync(code);
        }
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        ViewModel.StateChanged -= StateHasChanged;
    }

    private void LoginWithGitHub()
    {
        if (EnableGitHubAuth)
        {
            // Redirect to GitHub OAuth authorize URL
            NavigationManager.NavigateTo(GitHubAuthUrl, forceLoad: true);
        }
        else
        {
            ViewModel.ErrorMessage = "GitHub OAuth login requires additional configuration.";
        }
    }

    private string GetRedirectUri()
    {
        var baseUri = NavigationManager.BaseUri.TrimEnd('/');
        return Options.Value.Authentication.GitHubRedirectUri ??
               $"{baseUri}/osirion/auth/callback";
    }

    private async Task LoginWithGitHubCodeAsync(string code)
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoginWithGitHubAsync(code);
        });
    }

    private async Task LoginWithToken()
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoginWithTokenAsync();
        });
    }

    private void ToggleTokenInput()
    {
        ViewModel.ToggleTokenInput();
    }

    private string GetSanitizedReturnUrl()
    {
        // Sanitize and validate return URL to prevent open redirect
        if (string.IsNullOrWhiteSpace(ReturnUrl))
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
}
