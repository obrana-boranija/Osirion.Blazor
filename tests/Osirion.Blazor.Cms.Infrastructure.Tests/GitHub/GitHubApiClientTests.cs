using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using System.Net.Http.Headers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubApiClientTests
{
    private readonly RecordingHttpMessageHandler _handler = new();

    public static TheoryData<string, string> PathOperationsWithUnsafePaths()
    {
        var data = new TheoryData<string, string>();
        foreach (var operation in new[] { "read", "list", "write", "delete" })
        {
            foreach (var path in new[] { "../x.md", "a/../../o2/r2/contents/secret.md", "./a.md", "a//b.md", "a/.", @"..\x.md" })
            {
                data.Add(operation, path);
            }
        }

        return data;
    }

    [Fact]
    public async Task GetFileContentAsync_SendsTokenOnRequestMessage_AndNeverOnHttpClientDefaults()
    {
        // Proves the token travels per request: a token stored in DefaultRequestHeaders would be
        // shared with every other user of the same HttpClient.
        var httpClient = new HttpClient(_handler);
        var client = CreateClient(httpClient, token: "t1");

        await client.GetFileContentAsync("a.md");
        client.SetAccessToken("t2");

        httpClient.DefaultRequestHeaders.Authorization.ShouldBeNull();
        var request = _handler.Requests.ShouldHaveSingleItem();
        request.AuthorizationScheme.ShouldBe("token");
        request.AuthorizationParameter.ShouldBe("t1");
    }

    [Fact]
    public async Task ConcurrentRequests_FromTwoClientsSharingOneHttpClient_EachCarryOnlyTheirOwnToken()
    {
        // Proves two instances over one HttpClient never see each other's token, even when their
        // requests interleave.
        var httpClient = new HttpClient(_handler);
        var first = CreateClient(httpClient, token: "token-a");
        var second = CreateClient(httpClient, token: "token-b");

        var calls = Enumerable.Range(0, 20).SelectMany(index => new[]
        {
            first.GetFileContentAsync($"a-{index}.md"),
            second.GetFileContentAsync($"b-{index}.md")
        });
        await Task.WhenAll(calls);

        _handler.Requests.Count.ShouldBe(40);
        _handler.Requests.ShouldAllBe(request =>
            request.AuthorizationParameter == (request.Uri.AbsolutePath.Contains("/contents/a-") ? "token-a" : "token-b"));
    }

    [Fact]
    public async Task Constructor_KeepsUserAgentConfiguredOnHttpClient()
    {
        var httpClient = new HttpClient(_handler);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Consumer/1.0");
        var client = CreateClient(httpClient);

        client.SetAccessToken("t2");
        await client.GetFileContentAsync("a.md");

        _handler.Requests.ShouldHaveSingleItem().UserAgent.ShouldBe("Consumer/1.0");
    }

    [Fact]
    public async Task GetFileContentAsync_WithoutAccessToken_SendsNoRequest()
    {
        var client = CreateClient(token: string.Empty);

        var result = await client.GetFileContentAsync("a.md");

        result.ShouldBeNull();
        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteFileAsync_WithoutClientToken_SendsNoRequestWithTokenConfiguredOnHttpClient()
    {
        // Proves a delete without this instance's token fails before sending, so a token the host put on the
        // HttpClient's default headers can never perform it.
        var httpClient = new HttpClient(_handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", "host-token");
        var client = CreateClient(httpClient, token: string.Empty);

        await Should.ThrowAsync<InvalidOperationException>(() => client.DeleteFileAsync("a.md", "message", "sha"));

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFileContentAsync_WithPlainPathAndBranch_BuildsUnchangedUrl()
    {
        var client = CreateClient(branch: "feature/x");

        await client.GetFileContentAsync("content/blog/post-1_v2.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsoluteUri
            .ShouldBe("https://api.github.com/repos/o/r/contents/content/blog/post-1_v2.md?ref=feature/x");
    }

    [Fact]
    public async Task GetFileContentAsync_WithReservedCharactersInPath_KeepsThemInsideThePath()
    {
        // Proves '#' and '?' in a file name can no longer cut the path short or drop the branch.
        var client = CreateClient();

        await client.GetFileContentAsync("blog/my post #1?.md");

        var uri = _handler.Requests.ShouldHaveSingleItem().Uri;
        uri.AbsolutePath.ShouldBe("/repos/o/r/contents/blog/my%20post%20%231%3F.md");
        uri.Query.ShouldBe("?ref=main");
        uri.Fragment.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(PathOperationsWithUnsafePaths))]
    public async Task PathOperation_WithDotOrEmptySegment_ThrowsWithoutSendingRequest(string operation, string path)
    {
        // Proves a dot segment cannot retarget the request to another endpoint or repository with the
        // configured token: the request is refused before anything is sent.
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(() => InvokePathOperation(client, operation, path));

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_WithBackslashSeparators_TargetsTheFolderTheyName()
    {
        // Proves a path combined with System.IO.Path on Windows still addresses the folder it names, as it did
        // when System.Uri turned the backslashes into slashes.
        var client = CreateClient();

        await client.CreateOrUpdateFileAsync(@"localized\en\blog/my-file.md", "content", "message");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath
            .ShouldBe("/repos/o/r/contents/localized/en/blog/my-file.md");
    }

    [Fact]
    public async Task GetRepositoryContentsAsync_WithEmptyPath_ListsRepositoryRoot()
    {
        var client = CreateClient();

        await client.GetRepositoryContentsAsync(string.Empty);

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsoluteUri
            .ShouldBe("https://api.github.com/repos/o/r/contents/?ref=main");
    }

    [Fact]
    public async Task GetRepositoryContentsAsync_WithOuterSlashes_TrimsThem()
    {
        var client = CreateClient();

        await client.GetRepositoryContentsAsync("/content/");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/o/r/contents/content");
    }

    [Fact]
    public async Task GetFileContentAsync_WithReservedCharactersInBranch_KeepsThemInsideRefValue()
    {
        // Proves a branch name cannot add query parameters or a fragment to the request.
        var client = CreateClient(branch: "release&x=1#y");

        await client.GetFileContentAsync("a.md");

        var uri = _handler.Requests.ShouldHaveSingleItem().Uri;
        uri.Query.ShouldBe("?ref=release%26x%3D1%23y");
        uri.Fragment.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateBranchAsync_WithSlashInBranchName_KeepsBranchPathSegments()
    {
        var client = CreateClient();

        await client.CreateBranchAsync("feature/new", "main");

        _handler.Requests.Select(request => $"{request.Method} {request.Uri.AbsolutePath}").ShouldBe(new[]
        {
            "GET /repos/o/r/git/refs/heads/main",
            "POST /repos/o/r/git/refs",
            "GET /repos/o/r/branches/feature/new"
        });
    }

    [Theory]
    [InlineData("../x", "main")]
    [InlineData("x", "../main")]
    [InlineData("feature/", "main")]
    [InlineData("", "main")]
    public async Task CreateBranchAsync_WithUnsafeBranchName_ThrowsWithoutSendingRequest(string branchName, string baseBranch)
    {
        // Proves a branch name cannot retarget the ref lookup or the branch lookup.
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(() => client.CreateBranchAsync(branchName, baseBranch));

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task SetRepository_WithSeparatorsInOwner_KeepsOwnerInsideOneSegment()
    {
        // Proves an owner value cannot climb out of /repos/{owner}/{repo} into another repository.
        var client = CreateClient();
        client.SetRepository("o/../evil", "r");

        await client.GetFileContentAsync("a.md");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/repos/o%2F..%2Fevil/r/contents/a.md");
    }

    [Fact]
    public async Task SetRepository_WithDotSegmentRepository_ThrowsWithoutSendingRequest()
    {
        var client = CreateClient();
        client.SetRepository("o", "..");

        await Should.ThrowAsync<ArgumentException>(() => client.GetFileContentAsync("a.md"));

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetRepositoriesAsync_WithSeparatorsInOwner_KeepsOwnerInsideOneSegment()
    {
        // Proves an owner value cannot collapse the organization probe or the listing into another endpoint,
        // such as the token owner's own /user/repos.
        var client = CreateClient();
        client.SetRepository("o/../x", "r");

        await client.GetRepositoriesAsync();

        _handler.Requests.Select(request => request.Uri.AbsolutePath).ShouldBe(new[]
        {
            "/orgs/o%2F..%2Fx",
            "/orgs/o%2F..%2Fx/repos"
        });
    }

    [Fact]
    public async Task GetRepositoriesAsync_WithDotSegmentOwner_ThrowsWithoutSendingRequest()
    {
        var client = CreateClient();
        client.SetRepository("..", "r");

        await Should.ThrowAsync<ArgumentException>(() => client.GetRepositoriesAsync());

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchFilesAsync_WithQualifierCharactersInBranch_KeepsThemInsideBranchQualifier()
    {
        // Proves a branch name cannot add a search qualifier such as repo:other/private, or another query
        // parameter, to a search sent with the configured token.
        var client = CreateClient(branch: "a+repo:x/y&q=1");

        await client.SearchFilesAsync(string.Empty);

        _handler.Requests.ShouldHaveSingleItem().Uri.Query
            .ShouldBe("?q=repo:o/r+branch:a%2Brepo%3Ax/y%26q%3D1");
    }

    [Fact]
    public async Task SearchFilesAsync_WithPlainValues_BuildsUnchangedQuery()
    {
        var client = CreateClient();

        await client.SearchFilesAsync("hello world");

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsoluteUri
            .ShouldBe("https://api.github.com/search/code?q=repo:o/r+branch:main+hello%20world");
    }

    private GitHubApiClient CreateClient(HttpClient? httpClient = null, string token = "t1", string branch = "main")
    {
        var options = new GitHubOptions
        {
            Owner = "o",
            Repository = "r",
            Branch = branch,
            ApiToken = token
        };

        return new GitHubApiClient(
            httpClient ?? new HttpClient(_handler),
            Options.Create(options),
            NullLogger<GitHubApiClient>.Instance);
    }

    private static Task InvokePathOperation(GitHubApiClient client, string operation, string path)
    {
        return operation switch
        {
            "read" => client.GetFileContentAsync(path),
            "list" => client.GetRepositoryContentsAsync(path),
            "write" => client.CreateOrUpdateFileAsync(path, "content", "message"),
            "delete" => client.DeleteFileAsync(path, "message", "sha"),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
}
