using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

public class AuthenticationService
{
    private readonly IAuthenticationService _authService;
    private readonly IContentRepositoryAdapter _repositoryAdapter;

    public AuthenticationService(
        IAuthenticationService authService,
        IContentRepositoryAdapter repositoryAdapter)
    {
        _authService = authService;
        _repositoryAdapter = repositoryAdapter;
    }

    public bool IsAuthenticated => _authService.IsAuthenticated;

    public string? Username => _authService.Username;

    public async Task<bool> LoginWithTokenAsync(string token)
    {
        if (await _authService.SetAccessTokenAsync(token))
        {
            await _repositoryAdapter.SetAccessTokenAsync(token);
            return true;
        }

        return false;
    }

    public async Task<bool> LoginWithGitHubAsync(string code)
    {
        return await _authService.AuthenticateWithGitHubAsync(code);
    }

    public async Task SignOutAsync()
    {
        await _authService.SignOutAsync();
    }
}