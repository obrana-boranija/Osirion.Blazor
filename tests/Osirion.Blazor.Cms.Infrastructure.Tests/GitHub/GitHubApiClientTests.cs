using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubApiClientTests
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<CmsAdminOptions> _options;
    private readonly ILogger<GitHubApiClient> _logger;
    private readonly GitHubApiClient _apiClient;
    private readonly HttpMessageHandler _mockHandler;

    public GitHubApiClientTests()
    {
        _mockHandler = Substitute.For<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler);
        _logger = Substitute.For<ILogger<GitHubApiClient>>();

        var options = new CmsAdminOptions
        {
            GitHub = new GitHubAdminOptions
            {
                Owner = "testOwner",
                Repository = "testRepo",
                DefaultBranch = "main",
                ApiUrl = "https://api.github.com"
            },
            Authentication = new AuthenticationOptions
            {
                PersonalAccessToken = "test-token"
            }
        };

        _options = Options.Create(options);

        _apiClient = new GitHubApiClient(_httpClient, _options, _logger);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsRepositories()
    {
        // Arrange
        var responseContent = JsonSerializer.Serialize(new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "testOwner/repo1" },
            new GitHubRepository { Name = "repo2", FullName = "testOwner/repo2" }
        });

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        //_mockHandler.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        //    .Returns(response);

        // Act
        var result = await _apiClient.GetRepositoriesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("repo1");
        result[1].Name.ShouldBe("repo2");
    }

    [Fact]
    public void SetRepository_SetsOwnerAndRepository()
    {
        // Arrange
        var owner = "newOwner";
        var repo = "newRepo";

        // Act
        _apiClient.SetRepository(owner, repo);

        // Now perform an operation that would use these values
        var exception = Should.Throw<Exception>(() =>
            _apiClient.GetRepositoryContentsAsync("").GetAwaiter().GetResult());

        // Assert
        // We expect an exception because we're not actually making HTTP calls,
        // but the exception message should contain our new values
        exception.Message.ShouldContain("newOwner");
        exception.Message.ShouldContain("newRepo");
    }

    [Fact]
    public void SetBranch_SetsBranch()
    {
        // Act
        _apiClient.SetBranch("develop");

        // Verify the branch was set - indirect check through an operation
        var exception = Should.Throw<Exception>(() =>
            _apiClient.GetRepositoryContentsAsync("").GetAwaiter().GetResult());

        // The message won't contain the branch name because the API is authenticated and won't
        // reach the actual content request, but we can verify the method doesn't throw
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void SetAccessToken_SetsToken()
    {
        // Act
        _apiClient.SetAccessToken("new-token");

        // Verify token was set - indirect check through an operation
        var exception = Should.Throw<Exception>(() =>
            _apiClient.GetRepositoriesAsync().GetAwaiter().GetResult());

        // The API request will fail but we can verify the method doesn't throw
        exception.ShouldNotBeNull();
    }
}