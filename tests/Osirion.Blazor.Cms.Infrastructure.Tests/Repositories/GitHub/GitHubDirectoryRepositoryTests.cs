using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories.GitHub;

public class GitHubDirectoryRepositoryTests
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IOptions<GitHubOptions> _options;
    private readonly IDirectoryCacheManager _cacheManager;
    private readonly IDirectoryMetadataProcessor _metadataProcessor;
    private readonly IPathUtilities _pathUtils;
    private readonly ILogger<GitHubDirectoryRepository> _logger;
    private readonly GitHubDirectoryRepository _repository;

    public GitHubDirectoryRepositoryTests()
    {
        _apiClient = Substitute.For<IGitHubApiClient>();
        _cacheManager = Substitute.For<IDirectoryCacheManager>();
        _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
        _pathUtils = Substitute.For<IPathUtilities>();
        _logger = Substitute.For<ILogger<GitHubDirectoryRepository>>();

        var options = new GitHubOptions
        {
            Owner = "testOwner",
            Repository = "testRepo",
            Branch = "main",
            ContentPath = "content",
            ApiToken = "test-token",
            //CacheDurationMinutes = 5,
            EnableLocalization = false,
            DefaultLocale = "en"
        };

        _options = Options.Create(options);

        //_repository = new GitHubDirectoryRepository(
        //    _apiClient,
        //    _options,
        //    _cacheManager,
        //    _metadataProcessor,
        //    _pathUtils,
        //    _logger);

        SetupPathUtils();
    }

    private void SetupPathUtils()
    {
        _pathUtils.NormalizePath(Arg.Any<string>()).Returns(x => x.Arg<string>());
        _pathUtils.ExtractLocaleFromPath(Arg.Any<string>()).Returns("en");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCachedDirectories()
    {
        // Arrange
        var directories = new List<DirectoryItem>
        {
            DirectoryItem.Create("dir1", "content/dir1", "Dir 1", "github"),
            DirectoryItem.Create("dir2", "content/dir2", "Dir 2", "github")
        };

        var cachedDirs = new Dictionary<string, DirectoryItem>();
        foreach (var dir in directories)
        {
            cachedDirs[dir.Id] = dir;
        }

        _cacheManager.GetCachedDirectoriesAsync(
            Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<bool>())
            .Returns(cachedDirs);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldContain(item => item.Id == "dir1");
        result.ShouldContain(item => item.Id == "dir2");
    }

    [Fact]
    public async Task SaveAsync_CreatesNewDirectory()
    {
        // Arrange
        var directory = DirectoryItem.Create(
            "new-dir-id",
            "content/newdir",
            "New Directory",
            "github");

        // Response for placeholder file
        _apiClient.CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/newdir/.gitkeep"),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(s => s is null),
            Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse
            {
                Success = true,
                Content = new GitHubFileContent { Sha = "placeholder-sha" }
            });

        // Response for metadata file
        _apiClient.CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/newdir/_index.md"),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(s => s is null),
            Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse
            {
                Success = true,
                Content = new GitHubFileContent { Sha = "index-sha" }
            });

        _metadataProcessor.GenerateMetadataContent(directory).Returns("---\ntitle: New Directory\n---\n\n# New Directory\n");

        // Act
        var result = await _repository.SaveAsync(directory);

        // Assert
        result.ShouldNotBeNull();
        result.ProviderSpecificId.ShouldBe("index-sha");

        // Verify API calls
        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/newdir/.gitkeep"),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());

        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/newdir/_index.md"),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_RemovesDirectory()
    {
        // Arrange
        var directoryId = "dir-to-delete";
        var directory = DirectoryItem.Create(
            directoryId,
            "content/to-delete",
            "Dir to Delete",
            "github");

        

        var files = new List<GitHubItem>
        {
            new GitHubItem {
                Path = "content/to-delete/file1.md",
                Type = "file",
                Sha = "file1-sha"
            },
            new GitHubItem {
                Path = "content/to-delete/_index.md",
                Type = "file",
                Sha = "index-sha"
            }
        };

        _repository.GetByIdAsync(directoryId, Arg.Any<CancellationToken>())
            .Returns(directory);

        _apiClient.GetRepositoryContentsAsync(
            Arg.Is<string>(s => s == "content/to-delete"),
            Arg.Any<CancellationToken>())
            .Returns(files);

        _apiClient.DeleteFileAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = true });

        // Act
        await _repository.DeleteAsync(directoryId);

        // Assert
        // Verify files were deleted
        await _apiClient.Received(2).DeleteFileAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}