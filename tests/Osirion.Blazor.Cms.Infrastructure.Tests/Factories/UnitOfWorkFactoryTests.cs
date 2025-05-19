using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Factories;

public class UnitOfWorkFactoryTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IContentProviderRegistry _providerRegistry;
    private readonly IConfiguration _configuration;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILoggerFactory _loggerFactory;
    private readonly UnitOfWorkFactory _factory;

    private readonly IContentProvider _githubProvider;
    private readonly IContentProvider _filesystemProvider;
    private readonly IContentProvider _defaultProvider;
    private readonly IGitHubApiClient _apiClient;
    private readonly GitHubContentRepository _githubContentRepo;
    private readonly GitHubDirectoryRepository _githubDirectoryRepo;
    private readonly FileSystemContentRepository _filesystemContentRepo;
    private readonly FileSystemDirectoryRepository _filesystemDirectoryRepo;

    public UnitOfWorkFactoryTests()
    {
        // Setup mocks
        _serviceProvider = Substitute.For<IServiceProvider>();
        _providerRegistry = Substitute.For<IContentProviderRegistry>();
        _configuration = Substitute.For<IConfiguration>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _loggerFactory = Substitute.For<ILoggerFactory>();

        // Github components
        _githubProvider = Substitute.For<IContentProvider>();
        _apiClient = Substitute.For<IGitHubApiClient>();
        _githubContentRepo = Substitute.For<GitHubContentRepository>();
        _githubDirectoryRepo = Substitute.For<GitHubDirectoryRepository>();

        // Filesystem components
        _filesystemProvider = Substitute.For<IContentProvider>();
        _filesystemContentRepo = Substitute.For<FileSystemContentRepository>();
        _filesystemDirectoryRepo = Substitute.For<FileSystemDirectoryRepository>();

        // Default provider
        _defaultProvider = _githubProvider;

        // Setup provider IDs
        _githubProvider.ProviderId.Returns("github-test");
        _filesystemProvider.ProviderId.Returns("filesystem-test");

        // Setup provider registry
        _providerRegistry.GetProvider("github-test").Returns(_githubProvider);
        _providerRegistry.GetProvider("filesystem-test").Returns(_filesystemProvider);
        _providerRegistry.GetDefaultProvider().Returns(_defaultProvider);

        // Setup service provider
        _serviceProvider.GetRequiredService<IGitHubApiClient>().Returns(_apiClient);
        _serviceProvider.GetRequiredService<GitHubContentRepository>().Returns(_githubContentRepo);
        _serviceProvider.GetRequiredService<GitHubDirectoryRepository>().Returns(_githubDirectoryRepo);
        _serviceProvider.GetRequiredService<FileSystemContentRepository>().Returns(_filesystemContentRepo);
        _serviceProvider.GetRequiredService<FileSystemDirectoryRepository>().Returns(_filesystemDirectoryRepo);

        // Setup configuration
        _configuration["Osirion:Cms:FileSystem:BackupDirectory"]
            .Returns("/test/backup");

        // Setup logger
        var githubLogger = Substitute.For<ILogger<GitHubUnitOfWork>>();
        var filesystemLogger = Substitute.For<ILogger<FileSystemUnitOfWork>>();
        _loggerFactory.CreateLogger<GitHubUnitOfWork>().Returns(githubLogger);
        _loggerFactory.CreateLogger<FileSystemUnitOfWork>().Returns(filesystemLogger);

        // Create factory
        _factory = new UnitOfWorkFactory(
            _serviceProvider,
            _providerRegistry,
            _configuration,
            _eventDispatcher,
            _loggerFactory);
    }

    [Fact]
    public void Create_WithGitHubProvider_ReturnsGitHubUnitOfWork()
    {
        // Act
        var result = _factory.Create("github-test");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<GitHubUnitOfWork>();
        result.ProviderId.ShouldBe("github-test");
        result.ContentRepository.ShouldBe(_githubContentRepo);
        result.DirectoryRepository.ShouldBe(_githubDirectoryRepo);
    }

    [Fact]
    public void Create_WithFileSystemProvider_ReturnsFileSystemUnitOfWork()
    {
        // Act
        var result = _factory.Create("filesystem-test");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<FileSystemUnitOfWork>();
        result.ProviderId.ShouldBe("filesystem-test");
        result.ContentRepository.ShouldBe(_filesystemContentRepo);
        result.DirectoryRepository.ShouldBe(_filesystemDirectoryRepo);
    }

    [Fact]
    public void Create_WithUnsupportedProvider_ThrowsArgumentException()
    {
        // Arrange
        var unsupportedProvider = Substitute.For<IContentProvider>();
        unsupportedProvider.ProviderId.Returns("unsupported-provider");
        _providerRegistry.GetProvider("unsupported-provider").Returns(unsupportedProvider);

        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.Create("unsupported-provider"))
            .Message.ShouldContain("Unsupported provider type");
    }

    [Fact]
    public void Create_WithNonExistentProvider_ThrowsInvalidOperationException()
    {
        // Arrange
        _providerRegistry.GetProvider("non-existent").Returns((IContentProvider)null);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => _factory.Create("non-existent"))
            .Message.ShouldContain("Provider not found");
    }

    [Fact]
    public void CreateForDefaultProvider_ReturnsUnitOfWorkForDefaultProvider()
    {
        // Act
        var result = _factory.CreateForDefaultProvider();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<GitHubUnitOfWork>();
        result.ProviderId.ShouldBe("github-test");
    }

    [Fact]
    public void CreateForDefaultProvider_WithNoDefaultProvider_ThrowsInvalidOperationException()
    {
        // Arrange
        _providerRegistry.GetDefaultProvider().Returns((IContentProvider)null);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => _factory.CreateForDefaultProvider())
            .Message.ShouldContain("No default provider");
    }

    [Fact]
    public void Create_WithEmptyProviderId_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _factory.Create(string.Empty))
            .ParamName.ShouldBe("providerId");
    }

    [Fact]
    public void Create_WithFileSystemProvider_UsesBackupDirectoryFromConfiguration()
    {
        // Act
        var unitOfWork = _factory.Create("filesystem-test") as FileSystemUnitOfWork;

        // Assert
        unitOfWork.ShouldNotBeNull();

        // Check the backup directory via reflection
        var backupDirField = typeof(FileSystemUnitOfWork)
            .GetField("_backupDirectory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        backupDirField.ShouldNotBeNull();
        var backupDir = backupDirField.GetValue(unitOfWork) as string;
        backupDir.ShouldBe("/test/backup");
    }

    [Fact]
    public void Create_WithFileSystemProvider_UsesDefaultBackupDirectoryWhenNotConfigured()
    {
        // Arrange
        _configuration["Osirion:Cms:FileSystem:BackupDirectory"].Returns((string)null);

        // Act
        var unitOfWork = _factory.Create("filesystem-test") as FileSystemUnitOfWork;

        // Assert
        unitOfWork.ShouldNotBeNull();

        // Check the backup directory via reflection
        var backupDirField = typeof(FileSystemUnitOfWork)
            .GetField("_backupDirectory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        backupDirField.ShouldNotBeNull();
        var backupDir = backupDirField.GetValue(unitOfWork) as string;
        backupDir.ShouldContain("osirion-cms-backup");
    }
}