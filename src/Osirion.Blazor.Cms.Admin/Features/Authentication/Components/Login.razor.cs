using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Components;

public partial class Login
{
    [Inject]
    private IOptions<CmsAdminOptions> Options { get; set; } = default!;

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

    private string GitHubAuthUrl => $"https://github.com/login/oauth/authorize?client_id={Options.Value.Authentication.GitHubClientId}&redirect_uri={Uri.EscapeDataString(GetRedirectUri())}&scope=repo";

    protected override void OnInitialized()
    {
        ViewModel.StateChanged += StateHasChanged;
        ViewModel.ReturnUrl = ReturnUrl;

        // Check if GitHub OAuth is configured
        EnableGitHubAuth = !string.IsNullOrEmpty(Options.Value.Authentication.GitHubClientId) &&
                         !string.IsNullOrEmpty(Options.Value.Authentication.GitHubClientSecret);
    }

    protected override async Task OnInitializedAsync()
    {
        // Check if we have a code parameter (from GitHub OAuth redirect)
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        string code;
        if (queryParameters.TryGetValue("code", out code))
        {
            // Process GitHub OAuth login
            await LoginWithGitHubCodeAsync(code);
        }
    }

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
        return $"{baseUri}/admin/auth/callback";
    }

    private async Task LoginWithGitHubCodeAsync(string code)
    {
        await ExecuteAsync(async () =>
        {
            await ViewModel.LoginWithGitHubAsync(code);

            if (OnLoginResult.HasDelegate)
            {
                await OnLoginResult.InvokeAsync(string.IsNullOrEmpty(ViewModel.ErrorMessage));
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
                await OnLoginResult.InvokeAsync(string.IsNullOrEmpty(ViewModel.ErrorMessage));
            }
        });
    }
}