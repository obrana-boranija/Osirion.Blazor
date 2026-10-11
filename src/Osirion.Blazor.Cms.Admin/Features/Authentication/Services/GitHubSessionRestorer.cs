using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

/// <summary>Default <see cref="IGitHubSessionRestorer"/>.</summary>
public sealed class GitHubSessionRestorer : IGitHubSessionRestorer
{
    private readonly IAuthenticationService _authService;
    private readonly IGitHubTokenStore _tokenStore;
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly ILogger<GitHubSessionRestorer> _logger;

    /// <summary>Initializes a session restorer.</summary>
    public GitHubSessionRestorer(
        IAuthenticationService authService,
        IGitHubTokenStore tokenStore,
        IContentRepositoryAdapter repositoryAdapter,
        ILogger<GitHubSessionRestorer> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _repositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> RestoreAsync()
    {
        try
        {
            // Also removes any token an earlier version left in plain-text browser storage
            await _authService.InitializeAsync();

            if (_authService.IsAuthenticated)
            {
                return true;
            }

            var token = await _tokenStore.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            if (!await _authService.SetAccessTokenAsync(token))
            {
                _logger.LogWarning("The stored GitHub token is no longer valid and was removed");
                await _tokenStore.RemoveTokenAsync();
                return false;
            }

            await _repositoryAdapter.SetAccessTokenAsync(token);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not restore the GitHub login");
            return false;
        }
    }
}
