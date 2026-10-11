using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
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
/// Covers the commits <see cref="GitHubContentRepository"/> makes when it saves and deletes content, and the less
/// common paths of its cache load (listing fallback, localization, directories and unreadable files).
/// </summary>
public class GitHubContentRepositoryWriteTests
{
    private const string Provider = "github-test";

    private readonly IGitHubApiClient _client = Substitute.For<IGitHubApiClient>();
    private readonly IGitHubApiClientFactory _factory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IDirectoryRepository _directories = Substitute.For<IDirectoryRepository>();
    private readonly GitHubOptions _options = new()
    {
        Owner = "o",
        Repository = "r",
        Branch = "main",
        ContentPath = "content",
        ProviderId = Provider,
        SupportedExtensions = [".md"]
    };

    public GitHubContentRepositoryWriteTests()
    {
        _factory.GetDefaultClient().Returns(_client);
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns([]);
    }

    private GitHubContentRepository CreateRepository(string? providerName = null)
    {
        var processor = new MarkdownProcessor(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter());

        return new GitHubContentRepository(
            _factory, processor, Substitute.For<IContentQueryFilter>(), Options.Create(_options), _directories,
            NullLogger<GitHubContentRepository>.Instance, providerName);
    }

    private void SetTree(params (string Path, string Sha)[] files)
    {
        _client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns(new GitHubTree
        {
            Entries = files.Select(file => new GitHubTreeEntry { Path = file.Path, Sha = file.Sha, Type = "blob" }).ToList()
        });
        foreach (var file in files)
        {
            ServeFile(file.Path, file.Sha);
        }
    }

    private void ServeFile(string path, string sha, string? body = null)
    {
        var markdown = body ?? $"---\ntitle: {path}\n---\n# {path}\n";
        _client.GetFileContentAsync(path, Arg.Any<CancellationToken>()).Returns(new GitHubFileContent
        {
            Name = Path.GetFileName(path),
            Path = path,
            Sha = sha,
            Encoding = "base64",
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(markdown))
        });
    }

    private static ContentItem NewItem(string path, string? sha = null)
    {
        var item = ContentItem.Create("new-id", "New post", "Body text", path, Provider);
        if (sha is not null)
        {
            item.SetProviderSpecificId(sha);
        }

        return item;
    }

    private void RespondToSave(GitHubFileCommitResponse? response)
    {
        _client
            .CreateOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(response);
    }

    [Fact]
    public void Constructor_WithProviderNameAndNoToken_UsesNamedClientWithoutToken()
    {
        var named = Substitute.For<IGitHubApiClient>();
        _factory.GetClient("Docs").Returns(named);

        CreateRepository("Docs");

        named.Received(1).SetRepository("o", "r");
        named.Received(1).SetBranch("main");
        named.DidNotReceive().SetAccessToken(Arg.Any<string>());
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_WithNullEntity_Throws()
    {
        var repository = CreateRepository();

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveWithCommitMessageAsync(null!, "msg"));
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_WithoutPath_Throws()
    {
        var repository = CreateRepository();
        var item = ContentItem.Create("id", "Title", "Body", "content/x.md", Provider);
        item.Path = string.Empty;

        var exception = await Should.ThrowAsync<ArgumentException>(() => repository.SaveWithCommitMessageAsync(item, "msg"));

        exception.ParamName.ShouldBe("entity");
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_OnSuccess_CommitsFrontMatterAndStoresTheNewSha()
    {
        SetTree();
        RespondToSave(new GitHubFileCommitResponse { Success = true, Content = new GitHubFileContent { Sha = "new-sha" } });
        var repository = CreateRepository();
        var item = NewItem("content/new.md", "old-sha");

        var saved = await repository.SaveWithCommitMessageAsync(item, "Add a post");

        saved.ProviderSpecificId.ShouldBe("new-sha");
        await _client.Received(1).CreateOrUpdateFileAsync(
            "content/new.md",
            Arg.Is<string>(content => content.StartsWith("---") && content.Contains("New post")),
            "Add a post",
            "old-sha",
            Arg.Any<CancellationToken>());
        await _client.Received().GetTreeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_WhenGitHubReturnsNothing_ThrowsProviderException()
    {
        RespondToSave(null);
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => repository.SaveWithCommitMessageAsync(NewItem("content/new.md"), "msg"));

        exception.InnerException.ShouldBeOfType<ContentProviderException>().Message.ShouldContain("did not return a commit response");
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_WhenGitHubRejectsTheCommit_ThrowsWithItsMessage()
    {
        RespondToSave(new GitHubFileCommitResponse { Success = false, ErrorMessage = "sha mismatch" });
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => repository.SaveWithCommitMessageAsync(NewItem("content/new.md"), "msg"));

        exception.Message.ShouldContain("sha mismatch");
        exception.ProviderId.ShouldBe(Provider);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task DeleteWithCommitMessageAsync_WithEmptyId_Throws(string id)
    {
        var repository = CreateRepository();

        await Should.ThrowAsync<ArgumentException>(() => repository.DeleteWithCommitMessageAsync(id, "msg"));
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_WhenItemIsMissing_ThrowsNotFound()
    {
        SetTree();
        var repository = CreateRepository();

        var exception = await Should.ThrowAsync<ContentItemNotFoundException>(
            () => repository.DeleteWithCommitMessageAsync("missing", "msg"));

        exception.ContentId.ShouldBe("missing");
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_WhenItemHasNoSha_RefusesToDelete()
    {
        SetTree(("content/post.md", string.Empty));
        var repository = CreateRepository();
        var item = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => repository.DeleteWithCommitMessageAsync(item.Id, "msg"));

        exception.InnerException.ShouldBeOfType<ContentProviderException>().Message.ShouldContain("no SHA");
        await _client.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_OnSuccess_DeletesTheFileWithItsSha()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = true });
        var repository = CreateRepository();
        var item = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        await repository.DeleteWithCommitMessageAsync(item.Id, "Remove the post");

        await _client.Received(1).DeleteFileAsync("content/post.md", "Remove the post", "post-sha", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_WhenGitHubRejectsTheDelete_ThrowsWithItsMessage()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.DeleteFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = false, ErrorMessage = "conflict" });
        var repository = CreateRepository();
        var item = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => repository.DeleteWithCommitMessageAsync(item.Id, "msg"));

        exception.Message.ShouldContain("conflict");
    }

    [Fact]
    public async Task GetAllAsync_WithoutTree_ListsSubdirectoriesAndSkipsUnsupportedFiles()
    {
        _client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns((GitHubTree?)null);
        _client.GetRepositoryContentsAsync("content", Arg.Any<CancellationToken>()).Returns(
        [
            new GitHubItem { Path = "content/blog", Name = "blog", Type = "dir" },
            new GitHubItem { Path = "content/image.png", Name = "image.png", Type = "file" }
        ]);
        _client.GetRepositoryContentsAsync("content/blog", Arg.Any<CancellationToken>()).Returns(
        [
            new GitHubItem { Path = "content/blog/post.md", Name = "post.md", Type = "file", Sha = "post-sha" },
            new GitHubItem { Path = "content/blog/empty", Name = "empty", Type = "dir" }
        ]);
        _client.GetRepositoryContentsAsync("content/blog/empty", Arg.Any<CancellationToken>()).Returns((List<GitHubItem>?)null);
        ServeFile("content/blog/post.md", "post-sha");
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Path.ShouldBe("content/blog/post.md");
        await _client.DidNotReceive().GetFileContentAsync("content/image.png", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WithLocalization_AttachesTheDirectoryOfTheFile()
    {
        _options.EnableLocalization = true;
        _options.SupportedLocales = ["en", "de"];
        var directory = DirectoryItem.Create("dir", "content/de", "de", Provider);
        _directories.GetByPathAsync("content/de", Arg.Any<CancellationToken>()).Returns(directory);
        SetTree(("content/de/post.md", "post-sha"));
        var repository = CreateRepository();

        var item = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        item.Directory.ShouldBeSameAs(directory);
    }

    [Fact]
    public async Task GetAllAsync_WhenDirectoryLookupFails_StillLoadsTheItem()
    {
        _directories.GetByPathAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("directory cache unavailable"));
        SetTree(("content/post.md", "post-sha"));
        var repository = CreateRepository();

        var item = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        item.Directory.ShouldBeNull();
        item.Locale.ShouldBe("en");
    }

    [Fact]
    public async Task GetAllAsync_SkipsEmptyAndMissingFiles()
    {
        _client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns(new GitHubTree
        {
            Entries =
            [
                new GitHubTreeEntry { Path = "content/empty.md", Sha = "a", Type = "blob" },
                new GitHubTreeEntry { Path = "content/missing.md", Sha = "b", Type = "blob" }
            ]
        });
        ServeFile("content/empty.md", "a", body: "   ");
        _client.GetFileContentAsync("content/missing.md", Arg.Any<CancellationToken>()).Returns((GitHubFileContent?)null);
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenAFileCannotBeRead_SkipsOnlyThatFile()
    {
        SetTree(("content/good.md", "good-sha"), ("content/bad.md", "bad-sha"));
        _client.GetFileContentAsync("content/bad.md", Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("boom"));
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Path.ShouldBe("content/good.md");
    }

    [Fact]
    public async Task GetAllAsync_WhenAFileIsRateLimited_KeepsTheCacheEmptyAndWaitsForTheReset()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.GetFileContentAsync("content/post.md", Arg.Any<CancellationToken>())
            .ThrowsAsync(new GitHubRateLimitException(DateTimeOffset.UtcNow.AddHours(1), HttpStatusCode.Forbidden));
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();
        await repository.RefreshCacheAsync();

        items.ShouldBeEmpty();
        await _client.Received(1).GetTreeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WhenTheBranchListIsRateLimited_FailsTheLoad()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.GetBranchesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new GitHubRateLimitException(DateTimeOffset.UtcNow.AddHours(1), HttpStatusCode.TooManyRequests));
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenTheBranchListFails_StillLoadsItems()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("boom"));
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task GetAllAsync_WhenTheConfiguredBranchIsListed_LoadsItems()
    {
        SetTree(("content/post.md", "post-sha"));
        _client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new GitHubBranch { Name = "dev", Commit = new GitHubCommitRef { Sha = "dev-sha" } },
            new GitHubBranch { Name = "main", Commit = new GitHubCommitRef { Sha = "main-sha" } }
        ]);
        var repository = CreateRepository();

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem();
        await _client.Received(1).GetBranchesAsync(Arg.Any<CancellationToken>());
    }
}
