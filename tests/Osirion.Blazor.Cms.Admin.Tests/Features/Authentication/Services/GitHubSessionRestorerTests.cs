using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.Services;

public class GitHubSessionRestorerTests
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly GitHubSessionRestorer _restorer;

    public GitHubSessionRestorerTests()
    {
        _restorer = new GitHubSessionRestorer(_authService, _tokenStore, _adapter, NullLogger<GitHubSessionRestorer>.Instance);
    }

    [Fact]
    public async Task RestoreAsync_StoredValidToken_SignsInAndConfiguresTheAdapter()
    {
        _tokenStore.GetTokenAsync().Returns("stored-token");
        _authService.SetAccessTokenAsync("stored-token").Returns(true);

        (await _restorer.RestoreAsync()).ShouldBeTrue();

        await _adapter.Received(1).SetAccessTokenAsync("stored-token");
    }

    [Fact]
    public async Task RestoreAsync_AlreadyAuthenticated_DoesNotReadTheStore()
    {
        _authService.IsAuthenticated.Returns(true);

        (await _restorer.RestoreAsync()).ShouldBeTrue();

        await _tokenStore.DidNotReceive().GetTokenAsync();
    }

    [Fact]
    public async Task RestoreAsync_NoStoredToken_ReturnsFalse()
    {
        _tokenStore.GetTokenAsync().Returns((string?)null);

        (await _restorer.RestoreAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task RestoreAsync_TokenGitHubRejects_RemovesItAndReturnsFalse()
    {
        _tokenStore.GetTokenAsync().Returns("revoked");
        _authService.SetAccessTokenAsync("revoked").Returns(false);

        (await _restorer.RestoreAsync()).ShouldBeFalse();

        await _tokenStore.Received(1).RemoveTokenAsync();
        await _adapter.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
    }

    [Fact]
    public async Task RestoreAsync_StoreThrows_ReturnsFalseInsteadOfThrowing()
    {
        _tokenStore.GetTokenAsync().Returns(Task.FromException<string?>(new InvalidOperationException("storage")));

        (await _restorer.RestoreAsync()).ShouldBeFalse();
    }
}
