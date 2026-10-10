using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Keeps one pending GitHub login in memory with the single-use semantics of the real store.
/// </summary>
internal sealed class InMemoryGitHubOAuthStateStore : IGitHubOAuthStateStore
{
    public GitHubOAuthRequest? Pending { get; set; }

    public Task SaveAsync(GitHubOAuthRequest request)
    {
        Pending = request;
        return Task.CompletedTask;
    }

    public Task<GitHubOAuthRequest?> TakeAsync()
    {
        var pending = Pending;
        Pending = null;
        return Task.FromResult(pending);
    }
}
