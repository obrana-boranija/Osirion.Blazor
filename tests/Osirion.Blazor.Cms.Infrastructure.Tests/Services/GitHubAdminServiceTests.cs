using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class GitHubAdminServiceTests
{
    private const string AdminToken = "admin-pat";
    private const string UserToken = "user-token";

    private readonly RecordingHttpMessageHandler _handler = new();
    private readonly GitHubApiClientFactory _factory;

    public GitHubAdminServiceTests()
    {
        _factory = GitHubTestServices.CreateFactory(_handler);
    }

    [Fact]
    public async Task TwoInstances_KeepTokenRepositoryAndBranchToThemselves()
    {
        // Proves two admin sessions no longer share one client: neither sees the other's token,
        // repository or branch.
        var first = CreateService();
        var second = CreateService();
        await first.SetAuthTokenAsync("token-a");
        first.SetRepository("o1/r1");
        first.SetBranch("f1");
        await second.SetAuthTokenAsync("token-b");
        _handler.Clear();

        await first.GetFileContentAsync("x.md");
        await second.GetFileContentAsync("x.md");

        var requests = _handler.Requests;
        requests.Count.ShouldBe(2);
        requests[0].Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/o1/r1/contents/x.md?ref=f1");
        requests[0].AuthorizationParameter.ShouldBe("token-a");
        requests[1].Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/o/r/contents/x.md?ref=main");
        requests[1].AuthorizationParameter.ShouldBe("token-b");
    }

    [Fact]
    public async Task NewInstance_WithoutUserOrAdminToken_SendsNoRequest()
    {
        // Proves an admin session without a signed-in user no longer acts with the public provider's
        // server token.
        var service = CreateService();

        var items = await service.GetRepositoryContentsAsync("content");

        items.ShouldBeEmpty();
        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteFileAsync_WithoutUserOrAdminToken_SendsNoRequest()
    {
        // Proves a delete from an admin session without a token fails closed instead of sending an
        // unauthenticated DELETE that a token on the host's HttpClient could authorize.
        var service = CreateService();

        await Should.ThrowAsync<InvalidOperationException>(() => service.DeleteFileAsync("content/post.md", "message", "sha"));

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task NewInstance_WithConfiguredAdminToken_SendsAdminToken()
    {
        var service = CreateService(configuredAdminToken: AdminToken);

        await service.GetFileContentAsync("x.md");

        _handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe(AdminToken);
    }

    [Fact]
    public async Task SetProvider_KeepsUserTokenInsteadOfProviderToken()
    {
        // Proves switching provider neither drops the user's token nor picks up the provider's server token.
        var service = CreateService();
        await service.SetAuthTokenAsync("token-a");
        _handler.Clear();

        service.SetProvider(GitHubTestServices.DocsProvider);
        await service.GetFileContentAsync("x.md");

        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/o2/r2/contents/x.md?ref=main");
        request.AuthorizationParameter.ShouldBe("token-a");
    }

    [Fact]
    public async Task SetRepository_WithInvalidFormat_KeepsCurrentRepository()
    {
        var service = CreateService(configuredAdminToken: AdminToken);

        service.SetRepository("not-owner-slash-repo");
        await service.GetFileContentAsync("x.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/o/r/contents/x.md");
    }

    [Fact]
    public async Task NewInstance_WhileUserIsSignedIn_SendsUsersToken()
    {
        var authentication = CreateSignedInAuthentication();
        var service = CreateService(authentication: authentication);

        await service.GetFileContentAsync("x.md");

        _handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe(UserToken);
    }

    [Fact]
    public async Task SignIn_AfterConstruction_SendsUsersToken()
    {
        var authentication = Substitute.For<IAuthenticationService>();
        var service = CreateService(authentication: authentication);

        authentication.AccessToken.Returns(UserToken);
        authentication.AuthenticationChanged += Raise.Event<Action<bool>>(true);
        await service.GetFileContentAsync("x.md");

        _handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe(UserToken);
    }

    [Fact]
    public async Task SignOut_DropsUsersToken()
    {
        // Proves sign-out clears the token this scoped service holds, so it cannot outlive the session.
        var authentication = CreateSignedInAuthentication();
        var service = CreateService(authentication: authentication);
        await service.SetAuthTokenAsync(UserToken);
        _handler.Clear();

        authentication.AuthenticationChanged += Raise.Event<Action<bool>>(false);
        var items = await service.GetRepositoryContentsAsync("content");

        items.ShouldBeEmpty();
        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task SignOut_WithConfiguredAdminToken_FallsBackToAdminToken()
    {
        var authentication = CreateSignedInAuthentication();
        var service = CreateService(configuredAdminToken: AdminToken, authentication: authentication);

        authentication.AuthenticationChanged += Raise.Event<Action<bool>>(false);
        await service.GetFileContentAsync("x.md");

        _handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe(AdminToken);
    }

    private static IAuthenticationService CreateSignedInAuthentication()
    {
        var authentication = Substitute.For<IAuthenticationService>();
        authentication.IsAuthenticated.Returns(true);
        authentication.AccessToken.Returns(UserToken);
        return authentication;
    }

    private GitHubAdminService CreateService(
        string? configuredAdminToken = null,
        IAuthenticationService? authentication = null)
    {
        var options = new CmsAdminOptions
        {
            Authentication = new AuthenticationOptions
            {
                PersonalAccessToken = configuredAdminToken
            }
        };

        return new GitHubAdminService(
            _factory,
            Options.Create(options),
            Substitute.For<IMarkdownProcessor>(),
            NullLogger<GitHubAdminService>.Instance,
            authentication);
    }
}
