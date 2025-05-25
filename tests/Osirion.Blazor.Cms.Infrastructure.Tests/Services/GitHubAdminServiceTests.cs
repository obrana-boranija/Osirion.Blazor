using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class GitHubAdminServiceTests
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IOptions<CmsAdminOptions> _options;
    private readonly ILogger<GitHubAdminService> _logger;
    private readonly GitHubAdminService _adminService;

    public GitHubAdminServiceTests()
    {
        _apiClient = Substitute.For<IGitHubApiClient>();
        _logger = Substitute.For<ILogger<GitHubAdminService>>();

        var options = new CmsAdminOptions
        {
            //GitHub = new GitHubAdminOptions
            //{
            //    Owner = "testOwner",
            //    Repository = "testRepo",
            //    DefaultBranch = "main"
            //}
        };

        _options = Options.Create(options);

        //_adminService = new GitHubAdminService(
        //    _apiClient,
        //    _options,
        //    _logger);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsRepositories()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1" },
            new GitHubRepository { Name = "repo2" }
        };

        _apiClient.GetRepositoriesAsync().Returns(repositories);

        // Act
        var result = await _adminService.GetRepositoriesAsync();

        // Assert
        result.ShouldBe(repositories);
        await _apiClient.Received(1).GetRepositoriesAsync();
    }

    [Fact]
    public async Task CreateBranchAsync_CreatesBranch()
    {
        // Arrange
        var branchName = "feature/test";
        var fromBranch = "main";
        var branch = new GitHubBranch { Name = branchName };

        _apiClient.CreateBranchAsync(branchName, fromBranch, Arg.Any<CancellationToken>())
            .Returns(branch);

        // Act
        var result = await _adminService.CreateBranchAsync(branchName, fromBranch);

        // Assert
        result.ShouldBe(branch);
        await _apiClient.Received(1).CreateBranchAsync(
            branchName,
            fromBranch,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrUpdateFileAsync_CreatesFile()
    {
        // Arrange
        var path = "content/test.md";
        var content = "# Test Content";
        var message = "Create test file";
        var response = new GitHubFileCommitResponse
        {
            Success = true,
            Content = new GitHubFileContent { Sha = "new-sha" }
        };

        _apiClient.CreateOrUpdateFileAsync(
            path,
            content,
            message,
            null,
            Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _adminService.CreateOrUpdateFileAsync(path, content, message);

        // Assert
        result.ShouldBe(response);
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            path,
            content,
            message,
            null,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SetRepository_ConfiguresApiClient()
    {
        // Arrange
        var repository = "newOwner/newRepo";

        // Act
        _adminService.SetRepository(repository);

        // Assert
        _apiClient.Received(1).SetRepository("newOwner", "newRepo");
        _adminService.CurrentRepository.ShouldBe(repository);
    }

    [Fact]
    public void SetBranch_ConfiguresApiClient()
    {
        // Arrange
        var branch = "develop";

        // Act
        _adminService.SetBranch(branch);

        // Assert
        _apiClient.Received(1).SetBranch(branch);
        _adminService.CurrentBranch.ShouldBe(branch);
    }

    [Fact]
    public async Task SetAuthTokenAsync_SetsTokenOnApiClient()
    {
        // Arrange
        var token = "new-token";

        // Act
        await _adminService.SetAuthTokenAsync(token);

        // Assert
        _apiClient.Received(1).SetAccessToken(token);
    }
}