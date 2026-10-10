using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubRateLimitAndPaginationTests
{
    private static GitHubApiClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> respond, List<string>? requests = null)
    {
        var options = Options.Create(new GitHubOptions { Owner = "o", Repository = "r", Branch = "main", ApiToken = "token" });
        return new GitHubApiClient(new HttpClient(new StubHandler(respond, requests)), options, NullLogger<GitHubApiClient>.Instance);
    }

    private static HttpResponseMessage Json(string body, HttpStatusCode status = HttpStatusCode.OK) =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static string Branches(int from, int count) =>
        "[" + string.Join(",", Enumerable.Range(from, count).Select(i => $"{{\"name\":\"branch-{i}\",\"commit\":{{\"sha\":\"sha-{i}\"}}}}")) + "]";

    // Fails before the fix: only the first 30 branches were read, so a configured branch on a later page was missing.
    [Fact]
    public async Task GetBranchesAsync_FollowsPages_UntilAShortPage()
    {
        var requests = new List<string>();
        var client = CreateClient(request => request.RequestUri!.Query.EndsWith("&page=1")
            ? Json(Branches(0, 100))
            : Json(Branches(100, 5)), requests);

        var branches = await client.GetBranchesAsync();

        branches.ShouldNotBeNull().Count.ShouldBe(105);
        branches.ShouldContain(branch => branch.Name == "branch-104");
        requests.Count.ShouldBe(2);
    }

    // Fails before the fix: a rate-limit response surfaced as a generic HttpRequestException.
    [Fact]
    public async Task GetFileContentAsync_PrimaryRateLimit_ThrowsWithResetTime()
    {
        var reset = DateTimeOffset.UtcNow.AddMinutes(20).ToUnixTimeSeconds();
        var client = CreateClient(_ =>
        {
            var response = Json("{}", HttpStatusCode.Forbidden);
            response.Headers.Add("x-ratelimit-remaining", "0");
            response.Headers.Add("x-ratelimit-reset", reset.ToString());
            return response;
        });

        var exception = await Should.ThrowAsync<GitHubRateLimitException>(() => client.GetFileContentAsync("a.md"));

        exception.ResetAt.ShouldBe(DateTimeOffset.FromUnixTimeSeconds(reset));
    }

    [Fact]
    public async Task GetFileContentAsync_TooManyRequestsWithRetryAfter_ThrowsRateLimit()
    {
        var client = CreateClient(_ =>
        {
            var response = Json("{}", HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(90));
            return response;
        });

        var exception = await Should.ThrowAsync<GitHubRateLimitException>(() => client.GetFileContentAsync("a.md"));

        exception.ResetAt.ShouldNotBeNull().ShouldBeGreaterThan(DateTimeOffset.UtcNow.AddSeconds(60));
    }

    [Fact]
    public async Task GetFileContentAsync_PlainForbidden_IsNotARateLimit()
    {
        var client = CreateClient(_ => Json("{}", HttpStatusCode.Forbidden));

        var exception = await Should.ThrowAsync<HttpRequestException>(() => client.GetFileContentAsync("a.md"));

        exception.ShouldNotBeOfType<GitHubRateLimitException>();
    }

    // Fails before the fix: GitHub clients kept the 100 second framework default.
    [Fact]
    public void HttpClientPolicy_GitHubClient_GetsShortTimeout_ButAHostTimeoutIsKept()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(GitHubApiClientFactory.HttpClientNamePrefix + "a");
        services.AddHttpClient(GitHubApiClientFactory.HttpClientNamePrefix + "b")
            .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(5));
        services.AddHttpClient("Other");
        services.AddGitHubHttpClientCookiePolicy();
        using var provider = services.BuildServiceProvider();
        var factory = (IHttpClientFactory)provider.GetService(typeof(IHttpClientFactory))!;

        factory.CreateClient(GitHubApiClientFactory.HttpClientNamePrefix + "a").Timeout.ShouldBe(TimeSpan.FromSeconds(30));
        factory.CreateClient(GitHubApiClientFactory.HttpClientNamePrefix + "b").Timeout.ShouldBe(TimeSpan.FromSeconds(5));
        factory.CreateClient("Other").Timeout.ShouldBe(TimeSpan.FromSeconds(100));
    }

    // Repository behaviour on top of the tree, SHA reuse and rate limits.

    [Fact]
    public async Task Refresh_WithTree_DownloadsOnlyFilesWhoseShaChanged()
    {
        var harness = new RepositoryHarness();
        harness.SetTree(("content/a.md", "sha-a"), ("content/b.md", "sha-b"), ("other/c.md", "sha-c"), ("content/image.png", "sha-i"));

        await harness.Repository.RefreshCacheAsync();
        (await harness.Repository.GetAllAsync()).Count.ShouldBe(2);
        await harness.Client.Received(2).GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        harness.Client.ClearReceivedCalls();

        harness.SetTree(("content/a.md", "sha-a"), ("content/b.md", "sha-b2"), ("other/c.md", "sha-c"));
        await harness.Repository.RefreshCacheAsync();

        await harness.Client.Received(1).GetFileContentAsync("content/b.md", Arg.Any<CancellationToken>());
        await harness.Client.DidNotReceive().GetFileContentAsync("content/a.md", Arg.Any<CancellationToken>());
        await harness.Client.DidNotReceive().GetRepositoryContentsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        (await harness.Repository.GetAllAsync()).Count.ShouldBe(2);
    }

    [Fact]
    public async Task Refresh_WithoutTree_ListsDirectories_AndStillSkipsUnchangedFiles()
    {
        var harness = new RepositoryHarness();
        harness.Client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns((GitHubTree?)null);
        harness.Client.GetRepositoryContentsAsync("content", Arg.Any<CancellationToken>()).Returns(
            [new GitHubItem { Name = "a.md", Path = "content/a.md", Sha = "sha-a", Type = "file" }]);
        harness.ServeFile("content/a.md", "sha-a");

        await harness.Repository.RefreshCacheAsync();
        harness.Client.ClearReceivedCalls();
        await harness.Repository.RefreshCacheAsync();

        await harness.Client.DidNotReceive().GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        (await harness.Repository.GetAllAsync()).Count.ShouldBe(1);
    }

    // Fails before the fix: a failed load replaced the cache with an empty one.
    [Fact]
    public async Task Refresh_WhenRateLimited_KeepsTheCurrentCache_AndDoesNotCallAgainBeforeReset()
    {
        var harness = new RepositoryHarness();
        harness.SetTree(("content/a.md", "sha-a"));
        await harness.Repository.RefreshCacheAsync();
        harness.Client.ClearReceivedCalls();
        harness.Client.GetTreeAsync(Arg.Any<CancellationToken>())
            .Returns<Task<GitHubTree?>>(_ => throw new GitHubRateLimitException(DateTimeOffset.UtcNow.AddMinutes(10), HttpStatusCode.Forbidden));

        await harness.Repository.RefreshCacheAsync();
        await harness.Repository.RefreshCacheAsync();

        (await harness.Repository.GetAllAsync()).Count.ShouldBe(1);
        await harness.Client.Received(1).GetTreeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Refresh_WhenListingFails_KeepsTheCurrentCache()
    {
        var harness = new RepositoryHarness();
        harness.SetTree(("content/a.md", "sha-a"));
        await harness.Repository.RefreshCacheAsync();
        harness.Client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns((GitHubTree?)null);
        harness.Client.GetRepositoryContentsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<Task<List<GitHubItem>?>>(_ => throw new HttpRequestException("boom"));

        await harness.Repository.RefreshCacheAsync();

        (await harness.Repository.GetAllAsync()).Count.ShouldBe(1);
    }

    private sealed class RepositoryHarness
    {
        public IGitHubApiClient Client { get; } = Substitute.For<IGitHubApiClient>();

        public GitHubContentRepository Repository { get; }

        public RepositoryHarness()
        {
            var factory = Substitute.For<IGitHubApiClientFactory>();
            factory.GetDefaultClient().Returns(Client);
            var directories = Substitute.For<IDirectoryRepository>();
            var options = Options.Create(new GitHubOptions
            {
                Owner = "o",
                Repository = "r",
                Branch = "main",
                ContentPath = "content",
                ApiToken = "token",
                SupportedExtensions = [".md"]
            });
            var processor = new MarkdownProcessor(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter());

            Repository = new GitHubContentRepository(
                factory, processor, Substitute.For<IContentQueryFilter>(), options, directories,
                NullLogger<GitHubContentRepository>.Instance);
            Client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns([]);
        }

        public void SetTree(params (string Path, string Sha)[] files)
        {
            Client.GetTreeAsync(Arg.Any<CancellationToken>()).Returns(new GitHubTree
            {
                Entries = files.Select(file => new GitHubTreeEntry { Path = file.Path, Sha = file.Sha, Type = "blob" }).ToList()
            });
            foreach (var file in files)
            {
                ServeFile(file.Path, file.Sha);
            }
        }

        public void ServeFile(string path, string sha)
        {
            var body = Convert.ToBase64String(Encoding.UTF8.GetBytes($"---\ntitle: {path}\n---\n# {path}\n"));
            Client.GetFileContentAsync(path, Arg.Any<CancellationToken>()).Returns(new GitHubFileContent
            {
                Name = Path.GetFileName(path),
                Path = path,
                Sha = sha,
                Encoding = "base64",
                Content = body
            });
        }
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond, List<string>? requests) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            requests?.Add(request.RequestUri!.PathAndQuery);
            return Task.FromResult(respond(request));
        }
    }
}
