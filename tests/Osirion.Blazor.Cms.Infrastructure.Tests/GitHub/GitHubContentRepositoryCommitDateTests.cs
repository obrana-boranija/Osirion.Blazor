using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

/// <summary>
/// The dates of a page loaded by <see cref="GitHubContentRepository"/>: the front matter's <c>date</c> wins, and a page
/// without one takes the date of its file's last commit instead of the time of the load.
/// </summary>
public class GitHubContentRepositoryCommitDateTests
{
    private static readonly DateTime CommitDate = new(2025, 3, 14, 9, 30, 0, DateTimeKind.Utc);

    private readonly IGitHubApiClient _client = Substitute.For<IGitHubApiClient>();
    private readonly IGitHubApiClientFactory _factory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IDirectoryRepository _directories = Substitute.For<IDirectoryRepository>();

    public GitHubContentRepositoryCommitDateTests()
    {
        _factory.GetDefaultClient().Returns(_client);
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns([]);
    }

    private GitHubContentRepository CreateRepository()
    {
        var options = new GitHubOptions
        {
            Owner = "o",
            Repository = "r",
            Branch = "main",
            ContentPath = "content",
            ProviderId = "github-test",
            SupportedExtensions = [".md"]
        };
        var processor = new MarkdownProcessor(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter());

        return new GitHubContentRepository(
            _factory, processor, Substitute.For<IContentQueryFilter>(), Options.Create(options), _directories,
            NullLogger<GitHubContentRepository>.Instance);
    }

    private void Serve(string path, string sha, string markdown)
    {
        _client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns(new GitHubTree
        {
            Entries = [new GitHubTreeEntry { Path = path, Sha = sha, Type = "blob" }]
        });
        _client.GetFileContentAsync(path, Arg.Any<CancellationToken>()).Returns(new GitHubFileContent
        {
            Name = Path.GetFileName(path),
            Path = path,
            Sha = sha,
            Encoding = "base64",
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(markdown))
        });
    }

    [Fact]
    public async Task GetAllAsync_PageWithoutDate_TakesTheDateOfItsLastCommit()
    {
        // Fails before the fix: the page took the time of the load, so it moved on every restart.
        Serve("content/post.md", "sha-1", "---\ntitle: Post\n---\n# Post\n");
        _client.GetLastCommitDateAsync("content/post.md", Arg.Any<CancellationToken>()).Returns(CommitDate);

        var post = (await CreateRepository().GetAllAsync()).ShouldHaveSingleItem();

        post.DateCreated.ShouldBe(CommitDate);
        post.LastModified.ShouldBe(CommitDate);
    }

    [Fact]
    public async Task GetAllAsync_PageWithDate_KeepsItAndDoesNotAskForTheCommit()
    {
        Serve("content/post.md", "sha-1", "---\ntitle: Post\ndate: 2024-01-02\n---\n# Post\n");

        var post = (await CreateRepository().GetAllAsync()).ShouldHaveSingleItem();

        post.DateCreated.ShouldBe(new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        await _client.DidNotReceive().GetLastCommitDateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_CommitDateUnavailable_StillLoadsThePage()
    {
        Serve("content/post.md", "sha-1", "---\ntitle: Post\n---\n# Post\n");
        _client.GetLastCommitDateAsync("content/post.md", Arg.Any<CancellationToken>()).Throws(new HttpRequestException("500"));
        var before = DateTime.UtcNow;

        var post = (await CreateRepository().GetAllAsync()).ShouldHaveSingleItem();

        post.Title.ShouldBe("Post");
        post.DateCreated.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public async Task RefreshCacheAsync_FileUnchanged_DoesNotAskForTheCommitAgain()
    {
        Serve("content/post.md", "sha-1", "---\ntitle: Post\n---\n# Post\n");
        _client.GetLastCommitDateAsync("content/post.md", Arg.Any<CancellationToken>()).Returns(CommitDate);
        var repository = CreateRepository();
        await repository.GetAllAsync();

        await repository.RefreshCacheAsync();

        await _client.Received(1).GetLastCommitDateAsync("content/post.md", Arg.Any<CancellationToken>());
    }
}
