using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Factories;

/// <summary>
/// The factory resolves the concrete repositories from the service provider, and those cannot be substituted, so the
/// tests register real repositories built on substituted collaborators. The file system unit of work creates its backup
/// directory when it is constructed, so that directory lives in a temporary sandbox.
/// </summary>
public sealed class UnitOfWorkFactoryTests : IDisposable
{
    private const string BackupDirectoryKey = "Osirion:Cms:FileSystem:BackupDirectory";

    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-uow-factory-" + Guid.NewGuid().ToString("N"));
    private readonly IContentProviderRegistry _providerRegistry = Substitute.For<IContentProviderRegistry>();
    private readonly IDomainEventDispatcher _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
    private readonly Dictionary<string, string?> _configurationValues = new();
    private readonly ServiceProvider _serviceProvider;
    private readonly IContentProvider _githubProvider = Substitute.For<IContentProvider>();
    private readonly IContentProvider _filesystemProvider = Substitute.For<IContentProvider>();
    private readonly GitHubContentRepository _githubContentRepository;
    private readonly GitHubDirectoryRepository _githubDirectoryRepository;
    private readonly FileSystemContentRepository _filesystemContentRepository;
    private readonly FileSystemDirectoryRepository _filesystemDirectoryRepository;
    private readonly UnitOfWorkFactory _factory;
    private readonly string _backupDirectory;

    public UnitOfWorkFactoryTests()
    {
        _backupDirectory = Path.Combine(_sandbox, "backup");
        _configurationValues[BackupDirectoryKey] = _backupDirectory;

        var apiClientFactory = Substitute.For<IGitHubApiClientFactory>();
        apiClientFactory.GetDefaultClient().Returns(Substitute.For<IGitHubApiClient>());
        var gitHubOptions = Options.Create(new GitHubOptions { Owner = "testOwner", Repository = "testRepo", Branch = "main" });
        var fileSystemOptions = Options.Create(new FileSystemOptions
        {
            BasePath = Path.Combine(_sandbox, "content"),
            WatchForChanges = false
        });

        _githubDirectoryRepository = new GitHubDirectoryRepository(
            apiClientFactory,
            gitHubOptions,
            Substitute.For<IDirectoryCacheManager>(),
            Substitute.For<IDirectoryMetadataProcessor>(),
            Substitute.For<IPathUtilities>(),
            NullLogger<GitHubDirectoryRepository>.Instance);
        _githubContentRepository = new GitHubContentRepository(
            apiClientFactory,
            Substitute.For<IMarkdownProcessor>(),
            Substitute.For<IContentQueryFilter>(),
            gitHubOptions,
            _githubDirectoryRepository,
            NullLogger<GitHubContentRepository>.Instance);
        _filesystemContentRepository = new FileSystemContentRepository(
            Substitute.For<IMarkdownProcessor>(),
            Substitute.For<IContentQueryFilter>(),
            fileSystemOptions,
            NullLogger<FileSystemContentRepository>.Instance,
            new System.IO.Abstractions.FileSystem());
        _filesystemDirectoryRepository = new FileSystemDirectoryRepository(
            fileSystemOptions,
            Substitute.For<IDirectoryCacheManager>(),
            Substitute.For<IDirectoryMetadataProcessor>(),
            Substitute.For<IPathUtilities>(),
            NullLogger<FileSystemDirectoryRepository>.Instance);

        _serviceProvider = new ServiceCollection()
            .AddSingleton(Substitute.For<IGitHubApiClient>())
            .AddSingleton(_githubContentRepository)
            .AddSingleton(_githubDirectoryRepository)
            .AddSingleton(_filesystemContentRepository)
            .AddSingleton(_filesystemDirectoryRepository)
            .BuildServiceProvider();

        _githubProvider.ProviderId.Returns("github-test");
        _filesystemProvider.ProviderId.Returns("filesystem-test");
        _providerRegistry.GetProvider("github-test").Returns(_githubProvider);
        _providerRegistry.GetProvider("filesystem-test").Returns(_filesystemProvider);
        _providerRegistry.GetDefaultProvider().Returns(_githubProvider);

        _factory = CreateFactory();
    }

    public void Dispose()
    {
        _filesystemContentRepository.Dispose();
        _filesystemDirectoryRepository.Dispose();
        _githubContentRepository.Dispose();
        _githubDirectoryRepository.Dispose();
        _serviceProvider.Dispose();
        if (System.IO.Directory.Exists(_sandbox))
        {
            System.IO.Directory.Delete(_sandbox, recursive: true);
        }
    }

    [Fact]
    public void Create_WithGitHubProvider_ReturnsGitHubUnitOfWork()
    {
        // Act
        var result = _factory.Create("github-test");

        // Assert
        result.ShouldBeOfType<GitHubUnitOfWork>();
        result.ContentRepository.ShouldBeSameAs(_githubContentRepository);
        result.DirectoryRepository.ShouldBeSameAs(_githubDirectoryRepository);
    }

    [Fact]
    public void Create_WithFileSystemProvider_ReturnsFileSystemUnitOfWork()
    {
        // Act
        var result = _factory.Create("filesystem-test");

        // Assert
        result.ShouldBeOfType<FileSystemUnitOfWork>();
        result.ContentRepository.ShouldBeSameAs(_filesystemContentRepository);
        result.DirectoryRepository.ShouldBeSameAs(_filesystemDirectoryRepository);
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
        _providerRegistry.GetProvider("non-existent").Returns((IContentProvider)null!);

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
        result.ShouldBeOfType<GitHubUnitOfWork>();
        result.ContentRepository.ShouldBeSameAs(_githubContentRepository);
    }

    [Fact]
    public void CreateForDefaultProvider_WithNoDefaultProvider_ThrowsInvalidOperationException()
    {
        // Arrange
        _providerRegistry.GetDefaultProvider().Returns((IContentProvider)null!);

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
        var unitOfWork = _factory.Create("filesystem-test");

        // Assert
        ReadBackupDirectory(unitOfWork).ShouldBe(_backupDirectory);
        System.IO.Directory.Exists(_backupDirectory).ShouldBeTrue();
    }

    [Fact]
    public void Create_WithFileSystemProvider_UsesDefaultBackupDirectoryWhenNotConfigured()
    {
        // Arrange
        _configurationValues.Remove(BackupDirectoryKey);
        var factory = CreateFactory();

        // Act
        var unitOfWork = factory.Create("filesystem-test");

        // Assert
        ReadBackupDirectory(unitOfWork).ShouldBe(Path.Combine(Path.GetTempPath(), "osirion-cms-backup"));
    }

    private UnitOfWorkFactory CreateFactory()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(_configurationValues).Build();
        return new UnitOfWorkFactory(
            _serviceProvider,
            _providerRegistry,
            configuration,
            _eventDispatcher,
            NullLoggerFactory.Instance);
    }

    private static string ReadBackupDirectory(IUnitOfWork unitOfWork)
    {
        var field = typeof(FileSystemUnitOfWork).GetField("_backupDirectory", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The backup directory field was not found.");
        return field.GetValue(unitOfWork) as string
            ?? throw new InvalidOperationException("The backup directory was not initialized.");
    }
}
