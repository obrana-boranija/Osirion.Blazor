using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A <see cref="DirectoryRepositoryBase"/> whose load and delete are scripted by the test. The cache manager is a
/// substitute that runs the loader on every call, so each read sees <see cref="Directories"/> as they are.
/// </summary>
internal sealed class ScriptedDirectoryRepositoryBase : DirectoryRepositoryBase
{
    private ScriptedDirectoryRepositoryBase(IDirectoryCacheManager cacheManager, IPathUtilities pathUtilities, string defaultLocale)
        : base(
            "scripted",
            cacheManager,
            Substitute.For<IDirectoryMetadataProcessor>(),
            pathUtilities,
            Options.Create(new ContentProviderOptions { DefaultLocale = defaultLocale }),
            NullLogger.Instance)
    {
        CacheManagerSubstitute = cacheManager;
    }

    public IDirectoryCacheManager CacheManagerSubstitute { get; }

    public List<DirectoryItem> Directories { get; } = new();

    public Exception? LoadFailure { get; set; }

    public Exception? DeleteFailure { get; set; }

    public List<(string Id, bool Recursive, string? CommitMessage)> DeleteCalls { get; } = new();

    public static ScriptedDirectoryRepositoryBase Create(string defaultLocale = "en")
    {
        var cacheManager = Substitute.For<IDirectoryCacheManager>();
        cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(call => call.Arg<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>()(call.Arg<CancellationToken>()));

        return new ScriptedDirectoryRepositoryBase(cacheManager, new PathUtilities(Options.Create(new FileSystemOptions())), defaultLocale);
    }

    public override Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
        => Task.FromResult(entity);

    public override Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => DeleteRecursiveAsync(id, null, cancellationToken);

    protected override Task<Dictionary<string, DirectoryItem>> LoadDirectoriesAsync(CancellationToken cancellationToken)
    {
        if (LoadFailure is not null)
        {
            throw LoadFailure;
        }

        return Task.FromResult(Directories.ToDictionary(directory => directory.Id));
    }

    protected override Task DeleteDirectoryInternalAsync(string id, bool recursive, string? commitMessage, CancellationToken cancellationToken)
    {
        if (DeleteFailure is not null)
        {
            throw DeleteFailure;
        }

        DeleteCalls.Add((id, recursive, commitMessage));
        return Task.CompletedTask;
    }
}
