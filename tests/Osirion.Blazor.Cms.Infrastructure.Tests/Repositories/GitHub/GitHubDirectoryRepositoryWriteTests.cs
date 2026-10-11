using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories.GitHub;

/// <summary>
/// Covers the commits <see cref="GitHubDirectoryRepository"/> makes when it saves, moves and deletes directories.
/// </summary>
public class GitHubDirectoryRepositoryWriteTests
{
    private const string Provider = "github-test";

    private readonly IGitHubApiClient _apiClient = Substitute.For<IGitHubApiClient>();
    private readonly IGitHubApiClientFactory _factory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IDirectoryCacheManager _cacheManager = Substitute.For<IDirectoryCacheManager>();
    private readonly IDirectoryMetadataProcessor _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
    private readonly Dictionary<string, DirectoryItem> _cache = new();
    private readonly GitHubOptions _options = new()
    {
        Owner = "owner",
        Repository = "repo",
        Branch = "main",
        ContentPath = "content",
        ProviderId = Provider
    };

    public GitHubDirectoryRepositoryWriteTests()
    {
        _factory.GetDefaultClient().Returns(_apiClient);
        _cacheManager
            .GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(_ => _cache);
        _metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>()).Returns("metadata");
        _apiClient
            .CreateOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(call => new GitHubFileCommitResponse
            {
                Success = true,
                Content = new GitHubFileContent { Sha = "new-sha-" + call.ArgAt<string>(0) }
            });
        _apiClient
            .DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = true });
    }

    private GitHubDirectoryRepository CreateRepository(string? providerName = null, IPathUtilities? pathUtils = null)
    {
        return new GitHubDirectoryRepository(
            _factory,
            Options.Create(_options),
            _cacheManager,
            _metadataProcessor,
            pathUtils ?? Substitute.For<IPathUtilities>(),
            NullLogger<GitHubDirectoryRepository>.Instance,
            providerName);
    }

    private DirectoryItem AddCached(string id, string path, DirectoryItem? parent = null)
    {
        var directory = DirectoryItem.Create(id, path, path[(path.LastIndexOf('/') + 1)..], Provider);
        parent?.AddChild(directory);
        _cache[id] = directory;
        return directory;
    }

    private void SetListing(string path, params GitHubItem[] items)
    {
        _apiClient.GetRepositoryContentsAsync(path, Arg.Any<CancellationToken>()).Returns(items.ToList());
    }

    private void ServeFile(string path, string content)
    {
        _apiClient.GetFileContentAsync(path, Arg.Any<CancellationToken>())
            .Returns(new GitHubFileContent { Path = path, Content = content, Sha = path + "-sha" });
    }

    private static GitHubItem File(string path) => new() { Path = path, Name = path[(path.LastIndexOf('/') + 1)..], Type = "file", Sha = path + "-sha" };

    private static GitHubItem Dir(string path) => new() { Path = path, Name = path[(path.LastIndexOf('/') + 1)..], Type = "dir" };

    [Fact]
    public void Constructor_WithProviderNameAndNoToken_UsesNamedClientWithoutToken()
    {
        var namedClient = Substitute.For<IGitHubApiClient>();
        _factory.GetClient("Docs").Returns(namedClient);
        _options.ApiToken = string.Empty;

        CreateRepository("Docs");

        namedClient.Received(1).SetRepository("owner", "repo");
        namedClient.Received(1).SetBranch("main");
        namedClient.DidNotReceive().SetAccessToken(Arg.Any<string>());
        _factory.DidNotReceive().GetDefaultClient();
    }

    [Fact]
    public void Constructor_WithToken_SetsTokenOnDefaultClient()
    {
        _options.ApiToken = "token";

        CreateRepository();

        _apiClient.Received(1).SetAccessToken("token");
    }

    [Fact]
    public async Task SaveAsync_WithNullEntity_Throws()
    {
        var repository = CreateRepository();

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveAsync(null!));
    }

    [Fact]
    public async Task SaveAsync_ExistingDirectoryWithSha_UpdatesIndexFileOnly()
    {
        var directory = AddCached("blog", "content/blog");
        directory.SetProviderSpecificId("old-sha");
        var repository = CreateRepository();

        var saved = await repository.SaveAsync(directory);

        saved.ProviderSpecificId.ShouldBe("new-sha-content/blog/_index.md");
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/blog/_index.md", "metadata", "Update directory blog", "old-sha", Arg.Any<CancellationToken>());
        await _apiClient.DidNotReceive().CreateOrUpdateFileAsync(
            "content/blog/.gitkeep", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        await _apiClient.DidNotReceive().GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_NewDirectoryWithExistingIndexFile_ReusesItsSha()
    {
        var directory = DirectoryItem.Create("docs", "content/docs", "docs", Provider);
        ServeFile("content/docs/_index.md", "old");
        var repository = CreateRepository();

        await repository.SaveAsync(directory);

        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/docs/.gitkeep", string.Empty, "Create directory docs", null, Arg.Any<CancellationToken>());
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/docs/_index.md", "metadata", "Create directory docs", "content/docs/_index.md-sha", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_WhenIndexLookupFails_CreatesIndexFileWithoutSha()
    {
        var directory = DirectoryItem.Create("docs", "content/docs", "docs", Provider);
        _apiClient.GetFileContentAsync("content/docs/_index.md", Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("missing"));
        var repository = CreateRepository();

        await repository.SaveAsync(directory);

        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/docs/_index.md", "metadata", "Create directory docs", null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_WhenGitHubReturnsNoCommit_ThrowsProviderException()
    {
        var directory = AddCached("blog", "content/blog");
        _apiClient
            .CreateOrUpdateFileAsync("content/blog/_index.md", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns((GitHubFileCommitResponse?)null);
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.SaveAsync(directory));

        exception.InnerException.ShouldBeOfType<InvalidOperationException>();
        exception.ProviderId.ShouldBe(Provider);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task MoveAsync_WithEmptyId_Throws(string id)
    {
        var repository = CreateRepository();

        await Should.ThrowAsync<ArgumentException>(() => repository.MoveAsync(id, null));
    }

    [Fact]
    public async Task MoveAsync_WhenDirectoryIsMissing_WrapsNotFound()
    {
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("missing", null));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>().DirectoryId.ShouldBe("missing");
    }

    [Fact]
    public async Task MoveAsync_WhenNewParentIsMissing_WrapsNotFound()
    {
        AddCached("blog", "content/blog");
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("blog", "missing"));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>().DirectoryId.ShouldBe("missing");
    }

    [Fact]
    public async Task MoveAsync_IntoOwnDescendant_IsRejectedWithoutCommits()
    {
        var blog = AddCached("blog", "content/blog");
        AddCached("news", "content/blog/news", blog);
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("blog", "news"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        await _apiClient.DidNotReceive().CreateOrUpdateFileAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveAsync_UnderNewParent_CopiesEveryFileThenDeletesTheOriginal()
    {
        AddCached("a", "content/a");
        AddCached("b", "content/b");
        SetListing("content/a", File("content/a/one.md"), Dir("content/a/sub"), File("content/a/gone.md"));
        SetListing("content/a/sub", File("content/a/sub/two.md"));
        ServeFile("content/a/one.md", "first");
        ServeFile("content/a/sub/two.md", "second");
        _apiClient.GetFileContentAsync("content/a/gone.md", Arg.Any<CancellationToken>()).Returns((GitHubFileContent?)null);
        var repository = CreateRepository();

        var moved = await repository.MoveAsync("a", "b");

        moved.Path.ShouldBe("content/b/a");
        moved.Parent.ShouldNotBeNull().Id.ShouldBe("b");
        const string message = "Move directory from content/a to content/b/a";
        await _apiClient.Received(1).CreateOrUpdateFileAsync("content/b/a/one.md", "first", message, null, Arg.Any<CancellationToken>());
        await _apiClient.Received(1).CreateOrUpdateFileAsync("content/b/a/sub/two.md", "second", message, null, Arg.Any<CancellationToken>());
        await _apiClient.Received(1).DeleteFileAsync("content/a/one.md", message, "content/a/one.md-sha", Arg.Any<CancellationToken>());
        await _apiClient.Received(1).DeleteFileAsync("content/a/sub/two.md", message, "content/a/sub/two.md-sha", Arg.Any<CancellationToken>());
        await _apiClient.DidNotReceive().DeleteFileAsync("content/a/gone.md", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        // The directory is still cached, so its metadata is updated in place; the message follows the missing SHA.
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/b/a/_index.md", "metadata", "Create directory a", null, Arg.Any<CancellationToken>());
        await _apiClient.DidNotReceive().CreateOrUpdateFileAsync(
            "content/b/a/.gitkeep", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    // Before the fix the directory landed at the repository root ("news"), outside the content folder,
    // where the provider no longer loads it.
    public async Task MoveAsync_ToRoot_MovesTheDirectoryToTheTopOfTheContentFolder()
    {
        var blog = AddCached("blog", "content/blog");
        AddCached("news", "content/blog/news", blog);
        SetListing("content/blog/news", File("content/blog/news/a.md"));
        ServeFile("content/blog/news/a.md", "text");
        var pathUtils = Substitute.For<IPathUtilities>();
        pathUtils.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>().Trim('/'));
        var repository = CreateRepository(pathUtils: pathUtils);

        var moved = await repository.MoveAsync("news", null);

        moved.Path.ShouldBe("content/news");
        moved.Parent.ShouldBeNull();
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            "content/news/a.md", "text", "Move directory from content/blog/news to content/news", null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteRecursiveAsync_WithMessage_DeletesDeepestFilesFirstAndSkipsOtherEntries()
    {
        AddCached("blog", "content/blog");
        SetListing("content/blog",
            File("content/blog/top.md"),
            Dir("content/blog/deep"),
            new GitHubItem { Path = "content/blog/link", Name = "link", Type = "symlink" });
        SetListing("content/blog/deep", File("content/blog/deep/bottom.md"));
        var deleted = new List<string>();
        _apiClient
            .DeleteFileAsync(Arg.Do<string>(deleted.Add), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = true });
        var repository = CreateRepository();

        await repository.DeleteRecursiveAsync("blog", "Remove the blog");

        deleted.ShouldBe(["content/blog/deep/bottom.md", "content/blog/top.md"]);
        await _apiClient.Received(2).DeleteFileAsync(Arg.Any<string>(), "Remove the blog", Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithEmptyListing_CommitsNothing()
    {
        AddCached("blog", "content/blog");
        _apiClient.GetRepositoryContentsAsync("content/blog", Arg.Any<CancellationToken>()).Returns((List<GitHubItem>?)null);
        var repository = CreateRepository();

        await repository.DeleteAsync("blog");

        await _apiClient.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenDirectoryDisappearsBeforeTheDelete_ThrowsNotFound()
    {
        var blog = DirectoryItem.Create("blog", "content/blog", "blog", Provider);
        _cacheManager
            .GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(new Dictionary<string, DirectoryItem> { ["blog"] = blog }, new Dictionary<string, DirectoryItem>());
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<DirectoryNotFoundException>(() => repository.DeleteAsync("blog"));

        exception.DirectoryId.ShouldBe("blog");
    }
}
