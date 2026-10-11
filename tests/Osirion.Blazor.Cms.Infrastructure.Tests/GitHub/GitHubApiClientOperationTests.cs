using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using static Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles.ScriptedHttpMessageHandler;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

/// <summary>
/// Covers the requests <see cref="GitHubApiClient"/> sends for each operation, how it reads the answers and how it
/// behaves when it is not configured or GitHub fails.
/// </summary>
public class GitHubApiClientOperationTests
{
    private ScriptedHttpMessageHandler _handler = new(_ => Json("{}"));

    private GitHubApiClient CreateClient(
        Func<HttpRequestMessage, HttpResponseMessage> respond,
        string owner = "o",
        string repository = "r",
        string token = "t1",
        string branch = "main")
    {
        _handler = new ScriptedHttpMessageHandler(respond);
        var options = new GitHubOptions { Owner = owner, Repository = repository, Branch = branch, ApiToken = token };

        return new GitHubApiClient(new HttpClient(_handler), Options.Create(options), NullLogger<GitHubApiClient>.Instance);
    }

    private GitHubApiClient CreateUnconfiguredClient(string owner = "o", string token = "")
    {
        return CreateClient(_ => Json("{}"), owner: owner, token: token);
    }

    private static JsonElement BodyOf(ScriptedRequest request) => JsonDocument.Parse(request.Body!).RootElement;

    [Fact]
    public async Task GetRepositoriesAsync_WithoutToken_ThrowsWithoutSendingRequest()
    {
        var client = CreateUnconfiguredClient();

        await Should.ThrowAsync<UnauthorizedAccessException>(() => client.GetRepositoriesAsync());

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetRepositoriesAsync_ForAnOrganization_ListsTheOrganizationRepositories()
    {
        var client = CreateClient(request => request.RequestUri!.AbsolutePath == "/orgs/o"
            ? Json("{}")
            : Json("""[{"id":1,"name":"site"}]"""));

        var repositories = await client.GetRepositoriesAsync();

        repositories.ShouldNotBeNull().ShouldHaveSingleItem().Name.ShouldBe("site");
        _handler.Requests.Select(r => r.Uri.AbsolutePath).ShouldBe(["/orgs/o", "/orgs/o/repos"]);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ForAUser_ListsTheUserRepositories()
    {
        var client = CreateClient(request => request.RequestUri!.AbsolutePath == "/orgs/o"
            ? Json("{}", HttpStatusCode.NotFound)
            : Json("null"));

        var repositories = await client.GetRepositoriesAsync();

        repositories.ShouldNotBeNull().ShouldBeEmpty();
        _handler.Requests.Last().Uri.AbsolutePath.ShouldBe("/users/o/repos");
    }

    [Fact]
    public async Task GetRepositoriesAsync_WhenTheOrganizationCheckFails_ListsTheUserRepositories()
    {
        var client = CreateClient(request => request.RequestUri!.AbsolutePath == "/orgs/o"
            ? throw new HttpRequestException("network")
            : Json("[]"));

        await client.GetRepositoriesAsync();

        _handler.Requests.Last().Uri.AbsolutePath.ShouldBe("/users/o/repos");
    }

    [Fact]
    public async Task GetRepositoriesAsync_WithoutOwner_ListsTheAuthenticatedUserRepositories()
    {
        var client = CreateClient(_ => Json("[]"), owner: string.Empty);

        await client.GetRepositoriesAsync();

        _handler.Requests.ShouldHaveSingleItem().Uri.AbsolutePath.ShouldBe("/user/repos");
    }

    [Fact]
    public async Task GetRepositoriesAsync_WhenGitHubFails_Throws()
    {
        var client = CreateClient(_ => Json("{}", HttpStatusCode.InternalServerError), owner: string.Empty);

        await Should.ThrowAsync<HttpRequestException>(() => client.GetRepositoriesAsync());
    }

    [Fact]
    public async Task Operations_WithoutToken_ReturnNullWithoutSendingRequests()
    {
        var client = CreateUnconfiguredClient();

        (await client.GetBranchesAsync()).ShouldBeNull();
        (await client.GetRepositoryContentsAsync("content")).ShouldBeNull();
        (await client.CreateOrUpdateFileAsync("a.md", "text", "message")).ShouldBeNull();
        (await client.CreateBranchAsync("feature", "main")).ShouldBeNull();
        (await client.CreatePullRequestAsync("title", "body", "feature", "main")).ShouldBeNull();
        (await client.SearchFilesAsync("query")).ShouldBeNull();
        (await client.GetTreeAsync()).ShouldBeNull();
        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFileContentAsync_WithoutOwner_ReturnsNullWithoutSendingRequest()
    {
        var client = CreateUnconfiguredClient(owner: string.Empty, token: "t1");

        (await client.GetFileContentAsync("a.md")).ShouldBeNull();

        _handler.Requests.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetBranchesAsync_WhenGitHubFails_Throws()
    {
        var client = CreateClient(_ => Json("{}", HttpStatusCode.InternalServerError));

        await Should.ThrowAsync<HttpRequestException>(() => client.GetBranchesAsync());
    }

    [Fact]
    public async Task GetRepositoryContentsAsync_ForASingleFile_ReturnsOneItem()
    {
        var client = CreateClient(_ => Json("""{"name":"a.md","path":"a.md","type":"file"}"""));

        var items = await client.GetRepositoryContentsAsync("a.md");

        items.ShouldNotBeNull().ShouldHaveSingleItem().Path.ShouldBe("a.md");
    }

    [Fact]
    public async Task GetRepositoryContentsAsync_WithNullBody_ReturnsEmptyList()
    {
        var client = CreateClient(_ => Json("null"));

        var items = await client.GetRepositoryContentsAsync("content");

        items.ShouldNotBeNull().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFileContentAsync_WithNullBody_ReturnsEmptyContent()
    {
        var client = CreateClient(_ => Json("null"));

        var content = await client.GetFileContentAsync("a.md");

        content.ShouldNotBeNull().Path.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_SendsBase64ContentWithBranchAndSha()
    {
        var client = CreateClient(_ => Json("""{"content":{"sha":"new-sha"}}"""));

        var response = await client.CreateOrUpdateFileAsync("content/a.md", "hello", "Add a", "old-sha");

        response.ShouldNotBeNull().Content.Sha.ShouldBe("new-sha");
        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Method.ShouldBe(HttpMethod.Put);
        request.Uri.AbsolutePath.ShouldBe("/repos/o/r/contents/content/a.md");
        var body = BodyOf(request);
        body.GetProperty("message").GetString().ShouldBe("Add a");
        body.GetProperty("content").GetString().ShouldBe(Convert.ToBase64String(Encoding.UTF8.GetBytes("hello")));
        body.GetProperty("branch").GetString().ShouldBe("main");
        body.GetProperty("sha").GetString().ShouldBe("old-sha");
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_WithNullBody_Throws()
    {
        var client = CreateClient(_ => Json("null"));

        var exception = await Should.ThrowAsync<Exception>(() => client.CreateOrUpdateFileAsync("a.md", "text", "message"));

        exception.Message.ShouldContain("Failed to deserialize commit response for a.md");
    }

    [Fact]
    public async Task DeleteFileAsync_SendsShaMessageAndBranch()
    {
        var client = CreateClient(_ => Json("""{"commit":{"sha":"commit-sha"}}"""));

        var response = await client.DeleteFileAsync("content/a.md", "Remove a", "file-sha");

        response.Commit.Sha.ShouldBe("commit-sha");
        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Method.ShouldBe(HttpMethod.Delete);
        request.Uri.AbsolutePath.ShouldBe("/repos/o/r/contents/content/a.md");
        var body = BodyOf(request);
        body.GetProperty("message").GetString().ShouldBe("Remove a");
        body.GetProperty("sha").GetString().ShouldBe("file-sha");
        body.GetProperty("branch").GetString().ShouldBe("main");
    }

    [Fact]
    public async Task DeleteFileAsync_WithNullBody_Throws()
    {
        var client = CreateClient(_ => Json("null"));

        var exception = await Should.ThrowAsync<Exception>(() => client.DeleteFileAsync("a.md", "message", "sha"));

        exception.Message.ShouldContain("Failed to deserialize commit response for a.md");
    }

    [Fact]
    public async Task CreateBranchAsync_CreatesTheReferenceFromTheBaseBranchCommit()
    {
        var client = CreateClient(request => request.RequestUri!.AbsolutePath switch
        {
            "/repos/o/r/git/refs/heads/main" => Json("""{"ref":"refs/heads/main","object":{"sha":"base-sha"}}"""),
            "/repos/o/r/branches/feature" => Json("""{"name":"feature","commit":{"sha":"base-sha"}}"""),
            _ => Json("{}")
        });

        var branch = await client.CreateBranchAsync("feature", "main");

        branch.ShouldNotBeNull().Name.ShouldBe("feature");
        var create = _handler.Requests.Single(r => r.Method == HttpMethod.Post);
        create.Uri.AbsolutePath.ShouldBe("/repos/o/r/git/refs");
        BodyOf(create).GetProperty("ref").GetString().ShouldBe("refs/heads/feature");
        BodyOf(create).GetProperty("sha").GetString().ShouldBe("base-sha");
    }

    [Fact]
    public async Task CreateBranchAsync_WhenTheBaseReferenceHasNoObject_ThrowsBeforeCreating()
    {
        var client = CreateClient(_ => Json("""{"ref":"refs/heads/main"}"""));

        var exception = await Should.ThrowAsync<Exception>(() => client.CreateBranchAsync("feature", "main"));

        exception.Message.ShouldBe("Failed to get reference for branch main");
        _handler.Requests.ShouldNotContain(r => r.Method == HttpMethod.Post);
    }

    [Fact]
    public async Task CreateBranchAsync_WhenTheNewBranchCannotBeRead_Throws()
    {
        var client = CreateClient(request => request.RequestUri!.AbsolutePath.StartsWith("/repos/o/r/branches/", StringComparison.Ordinal)
            ? Json("null")
            : Json("""{"object":{"sha":"base-sha"}}"""));

        var exception = await Should.ThrowAsync<Exception>(() => client.CreateBranchAsync("feature", "main"));

        exception.Message.ShouldBe("Failed to get details for new branch feature");
    }

    [Fact]
    public async Task CreatePullRequestAsync_PostsTitleBodyHeadAndBase()
    {
        var client = CreateClient(_ => Json("""{"number":7,"title":"Update"}"""));

        var pullRequest = await client.CreatePullRequestAsync("Update", "Details", "feature", "main");

        pullRequest.ShouldNotBeNull().Number.ShouldBe(7);
        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Method.ShouldBe(HttpMethod.Post);
        request.Uri.AbsolutePath.ShouldBe("/repos/o/r/pulls");
        var body = BodyOf(request);
        body.GetProperty("title").GetString().ShouldBe("Update");
        body.GetProperty("body").GetString().ShouldBe("Details");
        body.GetProperty("head").GetString().ShouldBe("feature");
        body.GetProperty("base").GetString().ShouldBe("main");
    }

    [Fact]
    public async Task CreatePullRequestAsync_WithNullBody_Throws()
    {
        var client = CreateClient(_ => Json("null"));

        var exception = await Should.ThrowAsync<Exception>(() => client.CreatePullRequestAsync("t", "b", "h", "main"));

        exception.Message.ShouldBe("Failed to deserialize pull request response");
    }

    [Fact]
    public async Task SearchFilesAsync_WithoutBranchOrQuery_SearchesTheWholeRepository()
    {
        var client = CreateClient(_ => Json("null"), branch: string.Empty);

        var result = await client.SearchFilesAsync(string.Empty);

        result.ShouldNotBeNull().Items.ShouldBeEmpty();
        _handler.Requests.ShouldHaveSingleItem().Uri.Query.ShouldBe("?q=repo:o/r");
    }

    [Fact]
    public async Task SearchFilesAsync_WhenGitHubFails_Throws()
    {
        var client = CreateClient(_ => Json("{}", HttpStatusCode.UnprocessableEntity));

        await Should.ThrowAsync<HttpRequestException>(() => client.SearchFilesAsync("query"));
    }

    [Fact]
    public async Task GetTreeAsync_WithoutBranch_ReadsTheHeadTree()
    {
        var client = CreateClient(_ => Json("""{"sha":"tree-sha","truncated":false,"tree":[{"path":"a.md","type":"blob"}]}"""), branch: string.Empty);

        var tree = await client.GetTreeAsync();

        tree.ShouldNotBeNull().Entries.ShouldHaveSingleItem().Path.ShouldBe("a.md");
        _handler.Requests.ShouldHaveSingleItem().Uri.PathAndQuery.ShouldBe("/repos/o/r/git/trees/HEAD?recursive=1");
    }

    [Fact]
    public async Task GetTreeAsync_WhenGitHubFails_Throws()
    {
        var client = CreateClient(_ => Json("{}", HttpStatusCode.InternalServerError));

        await Should.ThrowAsync<HttpRequestException>(() => client.GetTreeAsync());
    }

    [Fact]
    public async Task Constructor_WithProviderOptions_UsesThePersonalAccessToken()
    {
        var handler = new RecordingHttpMessageHandler();
        var options = new GitHubProviderOptions
        {
            Owner = "o",
            Repository = "r",
            ApiToken = "api-token",
            Authentication = new AuthenticationOptions { PersonalAccessToken = "personal-token" }
        };
        var client = new GitHubApiClient(new HttpClient(handler), Options.Create<GitHubOptions>(options), NullLogger<GitHubApiClient>.Instance);

        await client.GetFileContentAsync("a.md");

        handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe("personal-token");
    }

    [Fact]
    public async Task Constructor_WithProviderOptionsWithoutPersonalToken_FallsBackToTheApiToken()
    {
        var handler = new RecordingHttpMessageHandler();
        var options = new GitHubProviderOptions { Owner = "o", Repository = "r", ApiToken = "api-token" };
        var client = new GitHubApiClient(new HttpClient(handler), Options.Create<GitHubOptions>(options), NullLogger<GitHubApiClient>.Instance);

        await client.GetFileContentAsync("a.md");

        handler.Requests.ShouldHaveSingleItem().AuthorizationParameter.ShouldBe("api-token");
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var options = Options.Create(new GitHubOptions());

        Should.Throw<ArgumentNullException>(() => new GitHubApiClient(null!, options, NullLogger<GitHubApiClient>.Instance));
        Should.Throw<ArgumentNullException>(() => new GitHubApiClient(new HttpClient(), options, null!));
    }
}
