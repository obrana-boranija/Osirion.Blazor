using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

/// <summary>
/// Covers how <see cref="GitHubAdminService"/> passes each operation to its own GitHub API client and what it returns
/// when GitHub answers with nothing or fails.
/// </summary>
public class GitHubAdminServiceDelegationTests
{
    private readonly IGitHubApiClient _client = Substitute.For<IGitHubApiClient>();
    private readonly IGitHubApiClientFactory _factory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IMarkdownProcessor _markdownProcessor = Substitute.For<IMarkdownProcessor>();

    public GitHubAdminServiceDelegationTests()
    {
        _factory.GetDefaultClient().Returns(_client);
    }

    private GitHubAdminService CreateService(IAuthenticationService? authenticationService = null)
    {
        return new GitHubAdminService(
            _factory,
            Options.Create(new CmsAdminOptions()),
            _markdownProcessor,
            NullLogger<GitHubAdminService>.Instance,
            authenticationService);
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var options = Options.Create(new CmsAdminOptions());
        var logger = NullLogger<GitHubAdminService>.Instance;

        Should.Throw<ArgumentNullException>(() => new GitHubAdminService(null!, options, _markdownProcessor, logger));
        Should.Throw<ArgumentNullException>(() => new GitHubAdminService(_factory, options, _markdownProcessor, null!));
    }

    [Fact]
    public void Constructor_WithoutAuthenticationService_ClearsTheProviderToken()
    {
        var service = new GitHubAdminService(
            _factory, Options.Create(new CmsAdminOptions()), _markdownProcessor, NullLogger<GitHubAdminService>.Instance);

        _client.Received(1).SetAccessToken(string.Empty);
        service.CurrentProvider.ShouldBeEmpty();
        service.CurrentBranch.ShouldBe("main");
    }

    [Fact]
    public void SetProvider_WithEmptyName_SwitchesToTheDefaultClient()
    {
        var service = CreateService();

        service.SetProvider(" ");

        service.CurrentProvider.ShouldBe("default");
        _factory.Received(2).GetDefaultClient();
    }

    [Fact]
    public async Task SetProvider_WithName_UsesThatProvidersClient()
    {
        var docsClient = Substitute.For<IGitHubApiClient>();
        _factory.GetClient("Docs").Returns(docsClient);
        var service = CreateService();

        service.SetProvider("Docs");
        await service.GetRepositoryContentsAsync("content");

        service.CurrentProvider.ShouldBe("Docs");
        await docsClient.Received(1).GetRepositoryContentsAsync("content", Arg.Any<CancellationToken>());
        await _client.DidNotReceive().GetRepositoryContentsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBranchAsync_ReturnsTheCreatedBranch()
    {
        var branch = new GitHubBranch { Name = "feature" };
        _client.CreateBranchAsync("feature", "main", Arg.Any<CancellationToken>()).Returns(branch);
        var service = CreateService();

        (await service.CreateBranchAsync("feature", "main")).ShouldBeSameAs(branch);
    }

    [Fact]
    public async Task CreateBranchAsync_WhenGitHubReturnsNothing_Throws()
    {
        _client.CreateBranchAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((GitHubBranch?)null);
        var service = CreateService();

        await Should.ThrowAsync<InvalidOperationException>(() => service.CreateBranchAsync("feature", "main"));
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_ReturnsTheCommit()
    {
        var commit = new GitHubFileCommitResponse { Success = true };
        _client.CreateOrUpdateFileAsync("a.md", "text", "Add a", "sha", Arg.Any<CancellationToken>()).Returns(commit);
        var service = CreateService();

        (await service.CreateOrUpdateFileAsync("a.md", "text", "Add a", "sha")).ShouldBeSameAs(commit);
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_WhenGitHubReturnsNothing_Throws()
    {
        _client.CreateOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns((GitHubFileCommitResponse?)null);
        var service = CreateService();

        await Should.ThrowAsync<InvalidOperationException>(() => service.CreateOrUpdateFileAsync("a.md", "text", "Add a"));
    }

    [Fact]
    public async Task CreatePullRequestAsync_ReturnsThePullRequest()
    {
        var pullRequest = new GitHubPullRequest { Number = 3 };
        _client.CreatePullRequestAsync("Title", "Body", "feature", "main", Arg.Any<CancellationToken>()).Returns(pullRequest);
        var service = CreateService();

        (await service.CreatePullRequestAsync("Title", "Body", "feature", "main")).ShouldBeSameAs(pullRequest);
    }

    [Fact]
    public async Task CreatePullRequestAsync_WhenGitHubReturnsNothing_Throws()
    {
        _client.CreatePullRequestAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((GitHubPullRequest?)null);
        var service = CreateService();

        await Should.ThrowAsync<InvalidOperationException>(() => service.CreatePullRequestAsync("Title", "Body", "feature", "main"));
    }

    [Fact]
    public async Task DeleteFileAsync_ReturnsTheCommit()
    {
        var commit = new GitHubFileCommitResponse { Success = true };
        _client.DeleteFileAsync("a.md", "Remove a", "sha", Arg.Any<CancellationToken>()).Returns(commit);
        var service = CreateService();

        (await service.DeleteFileAsync("a.md", "Remove a", "sha")).ShouldBeSameAs(commit);
    }

    [Fact]
    public async Task GetBlogPostAsync_ForMarkdownFile_ParsesFrontMatterAndContent()
    {
        var frontMatter = new FrontMatter { Title = "Hello" };
        _client.GetFileContentAsync("posts/hello.md", Arg.Any<CancellationToken>())
            .Returns(new GitHubFileContent { Path = "posts/hello.md", Sha = "sha-1", Content = "raw markdown" });
        _markdownProcessor.ExtractFrontMatterAndContent("raw markdown").Returns((frontMatter, "body"));
        var service = CreateService();

        var post = await service.GetBlogPostAsync("posts/hello.md");

        post.Path.ShouldBe("posts/hello.md");
        post.Sha.ShouldBe("sha-1");
        post.Metadata.ShouldBeSameAs(frontMatter);
        post.Content.ShouldBe("body");
    }

    [Fact]
    public async Task GetBlogPostAsync_ForOtherFile_KeepsOnlyPathAndSha()
    {
        _client.GetFileContentAsync("images/logo.png", Arg.Any<CancellationToken>())
            .Returns(new GitHubFileContent { Path = "images/logo.png", Sha = "sha-2", Content = "binary" });
        var service = CreateService();

        var post = await service.GetBlogPostAsync("images/logo.png");

        post.Path.ShouldBe("images/logo.png");
        post.Sha.ShouldBe("sha-2");
        post.Content.ShouldBeEmpty();
        _markdownProcessor.DidNotReceive().ExtractFrontMatterAndContent(Arg.Any<string>());
    }

    [Fact]
    public async Task GetBlogPostAsync_WhenTheFileIsMissing_ReturnsAnEmptyItem()
    {
        _client.GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((GitHubFileContent?)null);
        var service = CreateService();

        var post = await service.GetBlogPostAsync("missing.md");

        post.Path.ShouldBeEmpty();
        post.Sha.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFileContentAsync_WhenGitHubReturnsNothing_ThrowsNamingThePath()
    {
        _client.GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((GitHubFileContent?)null);
        var service = CreateService();

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => service.GetFileContentAsync("a.md"));

        exception.Message.ShouldContain("'a.md'");
    }

    [Fact]
    public async Task ListOperations_WhenGitHubReturnsNothing_ReturnEmptyLists()
    {
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns((List<GitHubBranch>?)null);
        _client.GetRepositoriesAsync(Arg.Any<CancellationToken>()).Returns((List<GitHubRepository>?)null);
        _client.SearchFilesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((GitHubSearchResult?)null);
        var service = CreateService();

        (await service.GetBranchesAsync("o/r")).ShouldBeEmpty();
        (await service.GetRepositoriesAsync()).ShouldBeEmpty();
        (await service.SearchFilesAsync("query")).ShouldBeEmpty();
    }

    [Fact]
    public async Task ListOperations_ReturnWhatGitHubReturns()
    {
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns([new GitHubBranch { Name = "main" }]);
        _client.GetRepositoriesAsync(Arg.Any<CancellationToken>()).Returns([new GitHubRepository { Name = "site" }]);
        _client.SearchFilesAsync("query", Arg.Any<CancellationToken>())
            .Returns(new GitHubSearchResult { Items = [new GitHubItem { Path = "a.md" }] });
        var service = CreateService();

        (await service.GetBranchesAsync("o/r")).ShouldHaveSingleItem().Name.ShouldBe("main");
        (await service.GetRepositoriesAsync()).ShouldHaveSingleItem().Name.ShouldBe("site");
        (await service.SearchFilesAsync("query")).ShouldHaveSingleItem().Path.ShouldBe("a.md");
    }

    public static TheoryData<string> FailingOperations() =>
    [
        "branch", "write", "pull", "delete", "post", "branches", "file", "repositories", "contents", "search"
    ];

    [Theory]
    [MemberData(nameof(FailingOperations))]
    public async Task Operations_WhenTheClientFails_RethrowTheSameException(string operation)
    {
        var failure = new HttpRequestException("GitHub is down");
        _client.CreateBranchAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.CreateOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.CreatePullRequestAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.GetRepositoriesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.GetRepositoryContentsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        _client.SearchFilesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(failure);
        var service = CreateService();

        Func<Task> act = operation switch
        {
            "branch" => () => service.CreateBranchAsync("feature", "main"),
            "write" => () => service.CreateOrUpdateFileAsync("a.md", "text", "message"),
            "pull" => () => service.CreatePullRequestAsync("t", "b", "h", "main"),
            "delete" => () => service.DeleteFileAsync("a.md", "message", "sha"),
            "post" => () => service.GetBlogPostAsync("a.md"),
            "branches" => () => service.GetBranchesAsync("o/r"),
            "file" => () => service.GetFileContentAsync("a.md"),
            "repositories" => () => service.GetRepositoriesAsync(),
            "contents" => () => service.GetRepositoryContentsAsync("content"),
            _ => () => service.SearchFilesAsync("query")
        };

        (await Should.ThrowAsync<HttpRequestException>(act)).ShouldBeSameAs(failure);
    }

    [Fact]
    public async Task SetAuthTokenAsync_WithEmptyToken_KeepsTheCurrentToken()
    {
        var service = CreateService();
        _client.ClearReceivedCalls();

        await service.SetAuthTokenAsync(" ");

        _client.DidNotReceive().SetAccessToken(Arg.Any<string>());
        await _client.DidNotReceive().GetRepositoriesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAuthTokenAsync_SetsTheTokenAndChecksIt()
    {
        var service = CreateService();

        await service.SetAuthTokenAsync("user-token");

        _client.Received(1).SetAccessToken("user-token");
        await _client.Received(1).GetRepositoriesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAuthTokenAsync_WhenTheCheckFails_KeepsTheToken()
    {
        _client.GetRepositoriesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("401"));
        var service = CreateService();

        await service.SetAuthTokenAsync("bad-token");

        _client.Received(1).SetAccessToken("bad-token");
    }

    [Fact]
    public void SetRepository_WithOwnerAndName_UpdatesTheClient()
    {
        var service = CreateService();

        service.SetRepository("owner/site");

        service.CurrentRepository.ShouldBe("owner/site");
        _client.Received(1).SetRepository("owner", "site");
    }

    [Fact]
    public void Dispose_StopsFollowingSignInChanges()
    {
        var authentication = Substitute.For<IAuthenticationService>();
        authentication.AccessToken.Returns("user-token");
        var service = CreateService(authentication);
        _client.ClearReceivedCalls();

        service.Dispose();
        authentication.AuthenticationChanged += Raise.Event<Action<bool>>(true);

        _client.DidNotReceive().SetAccessToken(Arg.Any<string>());
    }

    [Fact]
    public void Dispose_WithoutAuthenticationService_DoesNotThrow()
    {
        var service = CreateService();

        Should.NotThrow(service.Dispose);
    }
}
