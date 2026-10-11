using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Providers;

/// <summary>
/// Argument checks and initialization of <see cref="FileSystemContentProvider"/>. Initialization checks the base path on
/// the real file system, so it points at a temporary sandbox; content is still read from an in-memory file system.
/// </summary>
public sealed class FileSystemContentProviderInitializationTests : IDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-fs-provider-" + Guid.NewGuid().ToString("N"));
    private readonly IDirectoryCacheManager _directoryCacheManager = Substitute.For<IDirectoryCacheManager>();
    private readonly TaskCompletionSource _directoryRefreshed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly FileSystemOptions _options;
    private readonly FileSystemContentRepository _contentRepository;
    private readonly FileSystemDirectoryRepository _directoryRepository;
    private readonly FileSystemContentProvider _provider;

    public FileSystemContentProviderInitializationTests()
    {
        _options = new FileSystemOptions
        {
            BasePath = Path.Combine(_sandbox, "content"),
            CreateDirectoriesIfNotExist = true,
            WatchForChanges = false,
        };
        var options = Options.Create(_options);

        _directoryCacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(_ => new Dictionary<string, DirectoryItem>());
        _directoryCacheManager.InvalidateCacheAsync(Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                _directoryRefreshed.TrySetResult();
                return Task.CompletedTask;
            });

        _contentRepository = new FileSystemContentRepository(
            Substitute.For<IMarkdownProcessor>(),
            Substitute.For<IContentQueryFilter>(),
            options,
            NullLogger<FileSystemContentRepository>.Instance,
            new MockFileSystem());

        _directoryRepository = new FileSystemDirectoryRepository(
            options,
            _directoryCacheManager,
            Substitute.For<IDirectoryMetadataProcessor>(),
            Substitute.For<IPathUtilities>(),
            NullLogger<FileSystemDirectoryRepository>.Instance);

        _provider = new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            options,
            _memoryCache,
            NullLogger<FileSystemContentProvider>.Instance);
    }

    public void Dispose()
    {
        _contentRepository.Dispose();
        _directoryRepository.Dispose();
        _memoryCache.Dispose();
        if (System.IO.Directory.Exists(_sandbox))
        {
            System.IO.Directory.Delete(_sandbox, recursive: true);
        }
    }

    [Fact]
    public async Task InitializeAsync_BasePathMissingAndCreationAllowed_CreatesItAndPreloadsBothRepositories()
    {
        await _provider.InitializeAsync();

        System.IO.Directory.Exists(_options.BasePath).ShouldBeTrue();
        await _directoryRefreshed.Task.WaitAsync(Timeout);
    }

    [Fact]
    public async Task InitializeAsync_BasePathMissingAndCreationNotAllowed_DoesNotCreateItButStillPreloads()
    {
        _options.CreateDirectoriesIfNotExist = false;

        await _provider.InitializeAsync();

        await _directoryRefreshed.Task.WaitAsync(Timeout);
        System.IO.Directory.Exists(_options.BasePath).ShouldBeFalse();
    }

    [Fact]
    public async Task InitializeAsync_BasePathExists_LeavesItAndPreloads()
    {
        System.IO.Directory.CreateDirectory(_options.BasePath);

        await _provider.InitializeAsync();

        await _directoryRefreshed.Task.WaitAsync(Timeout);
        System.IO.Directory.Exists(_options.BasePath).ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task LookupMethods_BlankKey_ThrowArgumentException(string key)
    {
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetItemByPathAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetItemByUrlAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetDirectoryByPathAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetDirectoryByIdAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetDirectoryByUrlAsync(key));
    }
}
