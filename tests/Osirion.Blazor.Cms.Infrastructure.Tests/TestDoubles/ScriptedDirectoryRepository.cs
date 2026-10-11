using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>Options type for <see cref="ScriptedDirectoryRepository"/>; the base class only stores it.</summary>
internal sealed class ScriptedDirectoryOptions;

/// <summary>
/// A directory repository whose cache load, save and delete are scripted by the test, so the shared behavior of
/// <see cref="BaseDirectoryRepository{TOptions}"/> can be checked without a content source.
/// </summary>
internal sealed class ScriptedDirectoryRepository : BaseDirectoryRepository<ScriptedDirectoryOptions>
{
    public ScriptedDirectoryRepository(params DirectoryItem[] directories)
        : this("scripted", new ScriptedDirectoryOptions(), NullLogger.Instance)
    {
        Directories = directories.ToList();
    }

    public ScriptedDirectoryRepository(string providerId, ScriptedDirectoryOptions options, ILogger logger)
        : base(providerId, options, logger)
    {
    }

    public List<DirectoryItem> Directories { get; set; } = new();

    /// <summary>Gets or sets whether a load leaves the cache unset, as a provider that failed silently would.</summary>
    public bool LeaveCacheUnset { get; set; }

    public Exception? LoadFailure { get; set; }

    public Exception? SaveFailure { get; set; }

    public Exception? DeleteFailure { get; set; }

    public List<DirectoryItem> Saved { get; } = new();

    public List<(string Id, bool Recursive, string? CommitMessage)> DeleteCalls { get; } = new();

    protected override async Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        if (LoadFailure is not null)
        {
            throw LoadFailure;
        }

        if (LeaveCacheUnset || (DirectoryCache is not null && !forceRefresh))
        {
            return;
        }

        var cache = new Dictionary<string, DirectoryItem>();
        await LoadDirectoriesIntoCacheAsync(cache, cancellationToken);
        DirectoryCache = cache;
    }

    protected override Task LoadDirectoriesIntoCacheAsync(Dictionary<string, DirectoryItem> cache, CancellationToken cancellationToken)
    {
        foreach (var directory in Directories)
        {
            cache[directory.Id] = directory;
        }

        return Task.CompletedTask;
    }

    protected override Task<DirectoryItem> SaveDirectoryInternalAsync(DirectoryItem entity, CancellationToken cancellationToken)
    {
        if (SaveFailure is not null)
        {
            throw SaveFailure;
        }

        Saved.Add(entity);
        return Task.FromResult(entity);
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
