using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class GitHubRepositoryAdapterTests
{
    private readonly IGitHubAdminService _service = Substitute.For<IGitHubAdminService>();
    private readonly RecordingLogger<GitHubRepositoryAdapter> _logger = new();
    private readonly GitHubRepositoryAdapter _adapter;

    public GitHubRepositoryAdapterTests()
    {
        _adapter = new GitHubRepositoryAdapter(_service, _logger);
    }

    [Fact]
    public void Constructor_NullService_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new GitHubRepositoryAdapter(null!, _logger));
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsTheServiceResult()
    {
        var repositories = new List<GitHubRepository> { new() { Name = "site" } };
        _service.GetRepositoriesAsync().Returns(repositories);

        var result = await _adapter.GetRepositoriesAsync();

        result.ShouldBeSameAs(repositories);
        _logger.HasEntry(LogLevel.Debug, "GetRepositoriesAsync").ShouldBeTrue();
    }

    [Fact]
    public async Task GetRepositoriesAsync_ServiceFails_LogsAndRethrows()
    {
        var failure = new InvalidOperationException("boom");
        _service.GetRepositoriesAsync().ThrowsAsync(failure);

        var thrown = await Should.ThrowAsync<InvalidOperationException>(() => _adapter.GetRepositoriesAsync());

        thrown.ShouldBeSameAs(failure);
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception == failure && entry.Message.Contains("GetRepositoriesAsync"));
    }

    [Fact]
    public async Task GetBranchesAsync_ReturnsTheServiceResult()
    {
        var branches = new List<GitHubBranch> { new() { Name = "main" } };
        _service.GetBranchesAsync("site").Returns(branches);

        var result = await _adapter.GetBranchesAsync("site");

        result.ShouldBeSameAs(branches);
    }

    [Fact]
    public async Task GetBranchesAsync_ServiceFails_LogsTheRepositoryAndRethrows()
    {
        _service.GetBranchesAsync("site").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.GetBranchesAsync("site"));

        _logger.HasEntry(LogLevel.Error, "GetBranchesAsync", "repository: site").ShouldBeTrue();
    }

    [Fact]
    public async Task GetContentsAsync_ServiceFails_LogsThePathAndRethrows()
    {
        _service.GetRepositoryContentsAsync("docs").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.GetContentsAsync("docs"));

        _logger.HasEntry(LogLevel.Error, "GetContentsAsync", "path: docs").ShouldBeTrue();
    }

    [Fact]
    public async Task SearchFilesAsync_ReturnsTheServiceResult()
    {
        var items = new List<GitHubItem> { new() { Name = "post.md" } };
        _service.SearchFilesAsync("post").Returns(items);

        var result = await _adapter.SearchFilesAsync("post");

        result.ShouldBeSameAs(items);
    }

    [Fact]
    public async Task SearchFilesAsync_ServiceFails_LogsTheQueryAndRethrows()
    {
        _service.SearchFilesAsync("post").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.SearchFilesAsync("post"));

        _logger.HasEntry(LogLevel.Error, "SearchFilesAsync", "query: post").ShouldBeTrue();
    }

    [Fact]
    public async Task GetBlogPostAsync_ReturnsTheServiceResult()
    {
        var item = new ContentItem();
        _service.GetBlogPostAsync("blog/post.md").Returns(item);

        var result = await _adapter.GetBlogPostAsync("blog/post.md");

        result.ShouldBeSameAs(item);
    }

    [Fact]
    public async Task GetBlogPostAsync_ServiceFails_LogsThePathAndRethrows()
    {
        _service.GetBlogPostAsync("blog/post.md").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.GetBlogPostAsync("blog/post.md"));

        _logger.HasEntry(LogLevel.Error, "GetBlogPostAsync", "path: blog/post.md").ShouldBeTrue();
    }

    [Fact]
    public async Task SaveContentAsync_PassesEveryArgumentToTheService()
    {
        var response = new GitHubFileCommitResponse();
        _service.CreateOrUpdateFileAsync("a.md", "body", "msg", "sha1").Returns(response);

        var result = await _adapter.SaveContentAsync("a.md", "body", "msg", "sha1");

        result.ShouldBeSameAs(response);
    }

    [Fact]
    public async Task SaveContentAsync_ServiceFails_LogsThePathAndRethrows()
    {
        _service.CreateOrUpdateFileAsync("a.md", "body", "msg", null).ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.SaveContentAsync("a.md", "body", "msg"));

        _logger.HasEntry(LogLevel.Error, "SaveContentAsync", "path: a.md").ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteFileAsync_ReturnsTheServiceResult()
    {
        var response = new GitHubFileCommitResponse();
        _service.DeleteFileAsync("a.md", "msg", "sha1").Returns(response);

        var result = await _adapter.DeleteFileAsync("a.md", "msg", "sha1");

        result.ShouldBeSameAs(response);
    }

    [Fact]
    public async Task DeleteFileAsync_ServiceFails_LogsThePathAndRethrows()
    {
        _service.DeleteFileAsync("a.md", "msg", "sha1").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.DeleteFileAsync("a.md", "msg", "sha1"));

        _logger.HasEntry(LogLevel.Error, "DeleteFileAsync", "path: a.md").ShouldBeTrue();
    }

    [Fact]
    public async Task CreateBranchAsync_ReturnsTheServiceResultAndLogsBothBranches()
    {
        var branch = new GitHubBranch { Name = "feature" };
        _service.CreateBranchAsync("feature", "main").Returns(branch);

        var result = await _adapter.CreateBranchAsync("feature", "main");

        result.ShouldBeSameAs(branch);
        _logger.HasEntry(LogLevel.Debug, "CreateBranchAsync", "feature from main").ShouldBeTrue();
    }

    [Fact]
    public async Task CreateBranchAsync_ServiceFails_LogsBothBranchesAndRethrows()
    {
        _service.CreateBranchAsync("feature", "main").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.CreateBranchAsync("feature", "main"));

        _logger.HasEntry(LogLevel.Error, "name: feature, base: main").ShouldBeTrue();
    }

    [Fact]
    public async Task CreatePullRequestAsync_ReturnsTheServiceResult()
    {
        var pullRequest = new GitHubPullRequest();
        _service.CreatePullRequestAsync("t", "b", "feature", "main").Returns(pullRequest);

        var result = await _adapter.CreatePullRequestAsync("t", "b", "feature", "main");

        result.ShouldBeSameAs(pullRequest);
        _logger.HasEntry(LogLevel.Debug, "feature -> main").ShouldBeTrue();
    }

    [Fact]
    public async Task CreatePullRequestAsync_ServiceFails_LogsTheBranchesAndRethrows()
    {
        _service.CreatePullRequestAsync("t", "b", "feature", "main").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.CreatePullRequestAsync("t", "b", "feature", "main"));

        _logger.HasEntry(LogLevel.Error, "head: feature, base: main").ShouldBeTrue();
    }

    [Fact]
    public void SetRepository_ForwardsToTheService()
    {
        _adapter.SetRepository("site");

        _service.Received(1).SetRepository("site");
    }

    [Fact]
    public void SetRepository_ServiceFails_LogsAndRethrows()
    {
        _service.When(service => service.SetRepository("site")).Do(_ => throw new InvalidOperationException("boom"));

        Should.Throw<InvalidOperationException>(() => _adapter.SetRepository("site"));

        _logger.HasEntry(LogLevel.Error, "SetRepository", "repository: site").ShouldBeTrue();
    }

    [Fact]
    public void SetBranch_ForwardsToTheService()
    {
        _adapter.SetBranch("main");

        _service.Received(1).SetBranch("main");
    }

    [Fact]
    public void SetBranch_ServiceFails_LogsAndRethrows()
    {
        _service.When(service => service.SetBranch("main")).Do(_ => throw new InvalidOperationException("boom"));

        Should.Throw<InvalidOperationException>(() => _adapter.SetBranch("main"));

        _logger.HasEntry(LogLevel.Error, "SetBranch", "branch: main").ShouldBeTrue();
    }

    [Fact]
    public async Task SetAccessTokenAsync_ForwardsToTheServiceWithoutLoggingTheToken()
    {
        await _adapter.SetAccessTokenAsync("ghp_secret");

        await _service.Received(1).SetAuthTokenAsync("ghp_secret");
        _logger.Entries.ShouldNotContain(entry => entry.Message.Contains("ghp_secret"));
    }

    [Fact]
    public async Task SetAccessTokenAsync_ServiceFails_LogsAndRethrows()
    {
        _service.SetAuthTokenAsync("token").ThrowsAsync(new InvalidOperationException("boom"));

        await Should.ThrowAsync<InvalidOperationException>(() => _adapter.SetAccessTokenAsync("token"));

        _logger.HasEntry(LogLevel.Error, "SetAccessTokenAsync").ShouldBeTrue();
    }
}
