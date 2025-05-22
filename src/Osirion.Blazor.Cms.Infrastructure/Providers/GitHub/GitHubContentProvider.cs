using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using System.Collections.Concurrent;

namespace Osirion.Blazor.Cms.Infrastructure.Providers;

public class GitHubContentProvider : ContentProviderBase, IContentCacheUpdater
{
    private readonly GitHubContentRepository _contentRepository;
    private readonly GitHubDirectoryRepository _directoryRepository;
    private readonly GitHubOptions _options;
    private readonly IGitHubApiClient _apiClient;
    private readonly SemaphoreSlim _updateLock = new(1, 1);
    private bool _updateInProgress = false;
    private string? _lastKnownSha = null;

    // Queue for handling multiple update requests
    private readonly ConcurrentQueue<string> _pendingShaTasks = new();
    private bool _processingQueue = false;

    public GitHubContentProvider(
        GitHubContentRepository contentRepository,
        GitHubDirectoryRepository directoryRepository,
        IOptions<GitHubOptions> options,
        IGitHubApiClient apiClient,
        IMemoryCache memoryCache,
        ILogger<GitHubContentProvider> logger)
        : base(memoryCache, options, logger)
    {
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        // Initialize with current SHA if possible
        Task.Run(async () =>
        {
            try
            {
                var branches = await _apiClient.GetBranchesAsync();
                var branch = branches.FirstOrDefault(b => b.Name == _options.Branch);
                if (branch != null)
                {
                    _lastKnownSha = branch.Commit.Sha;
                    Logger.LogInformation("Initialized with SHA {Sha} for branch {Branch}",
                        _lastKnownSha, _options.Branch);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to get initial commit SHA");
            }
        });
    }

    public override string ProviderId => _options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}";

    public override string DisplayName => $"GitHub: {_options.Owner}/{_options.Repository}";

    public override bool IsReadOnly => string.IsNullOrEmpty(_options.ApiToken);

    public override async Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _contentRepository.GetAllAsync(cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _contentRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _contentRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _contentRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        return await _contentRepository.FindByQueryAsync(query, cancellationToken);
    }

    public override async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        return await _directoryRepository.GetByLocaleAsync(locale, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        return await _directoryRepository.GetByPathAsync(path, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _directoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public override async Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        return await _directoryRepository.GetByUrlAsync(url, cancellationToken);
    }

    public override async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group categories
        return allContent
            .SelectMany(item => item.Categories)
            .GroupBy(category => category.ToLowerInvariant())
            .Select(group => ContentCategory.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(c => c.Name)
            .ToList();
    }

    public override async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        // Get all content items
        var allContent = await GetAllItemsAsync(cancellationToken);

        // Extract and group tags
        return allContent
            .SelectMany(item => item.Tags)
            .GroupBy(tag => tag.ToLowerInvariant())
            .Select(group => ContentTag.Create(
                group.First(),
                slug: group.First().GenerateSlug(),
                count: group.Count()))
            .OrderBy(t => t.Name)
            .ToList();
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Initializing GitHub content provider: {Owner}/{Repository}",
            _options.Owner, _options.Repository);

        // Configure API client
        _apiClient.SetRepository(_options.Owner, _options.Repository);
        _apiClient.SetBranch(_options.Branch);

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _apiClient.SetAccessToken(_options.ApiToken);
        }

        // Get the latest commit SHA
        try
        {
            var branches = await _apiClient.GetBranchesAsync(cancellationToken);
            var branch = branches.FirstOrDefault(b => b.Name == _options.Branch);
            if (branch != null)
            {
                _lastKnownSha = branch.Commit.Sha;
                Logger.LogInformation("Latest commit SHA for branch {Branch}: {Sha}",
                    _options.Branch, _lastKnownSha);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to get latest commit SHA during initialization");
        }

        // Pre-load some data in the background
        _ = Task.Run(async () => {
            try
            {
                await _contentRepository.RefreshCacheAsync(cancellationToken);
                await _directoryRepository.RefreshCacheAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error pre-loading content for GitHub provider");
            }
        }, cancellationToken);

        await base.InitializeAsync(cancellationToken);
    }

    public override async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        // Directly call repositories without acquiring our own semaphore
        await _contentRepository.RefreshCacheAsync(cancellationToken);
        await _directoryRepository.RefreshCacheAsync(cancellationToken);

        // Call base class implementation which might handle memory cache
        await base.RefreshCacheAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the content cache with the latest changes from GitHub
    /// </summary>
    public async Task UpdateCacheAsync(string? latestSha = null, bool forceBackground = false)
    {
        // Skip if the SHA hasn't changed and not forcing update
        if (!string.IsNullOrEmpty(latestSha) &&
            !string.IsNullOrEmpty(_lastKnownSha) &&
            latestSha == _lastKnownSha &&
            !forceBackground)
        {
            Logger.LogDebug("No changes detected (SHA unchanged), skipping cache update");
            return;
        }

        // Add to queue instead of processing immediately
        if (!string.IsNullOrEmpty(latestSha))
        {
            _pendingShaTasks.Enqueue(latestSha);
            Logger.LogInformation("Added SHA {Sha} to update queue for provider: {ProviderId}",
                latestSha, ProviderId);
        }

        // Start processing if not already doing so
        if (!_processingQueue)
        {
            await StartProcessingQueueAsync(forceBackground || _options.UseBackgroundCacheUpdate);
        }
    }

    private async Task StartProcessingQueueAsync(bool useBackground)
    {
        // Only attempt to acquire the lock if we're not already processing
        if (_processingQueue) return;

        // Try to acquire the semaphore - if we can't get it, someone else is already processing
        bool lockTaken = false;
        try
        {
            lockTaken = await _updateLock.WaitAsync(0);
            if (!lockTaken)
            {
                Logger.LogDebug("Another thread is already processing the queue");
                return;
            }

            // Double-check inside the lock
            if (_processingQueue)
            {
                // Someone else started processing while we were waiting
                return;
            }

            _processingQueue = true;

            // Process queue in the background if requested
            if (useBackground)
            {
                // Start background processing and release the current lock
                // as the background task will acquire its own lock
                _updateLock.Release();
                lockTaken = false;

                // Fire and forget in the background
                _ = Task.Run(async () =>
                {
                    bool bgLockAcquired = false;
                    try
                    {
                        // Acquire the lock for background processing
                        await _updateLock.WaitAsync();
                        bgLockAcquired = true;

                        await ProcessQueueInternalAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error in background queue processing");
                    }
                    finally
                    {
                        _processingQueue = false;

                        // Only release if we successfully acquired
                        if (bgLockAcquired)
                        {
                            _updateLock.Release();
                        }
                    }
                });
            }
            else
            {
                try
                {
                    // We already have the lock, so process synchronously
                    await ProcessQueueInternalAsync();
                }
                finally
                {
                    _processingQueue = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting queue processing");
            _processingQueue = false;
        }
        finally
        {
            // Only release if we acquired and didn't already release
            if (lockTaken)
            {
                _updateLock.Release();
            }
        }
    }

    private async Task ProcessQueueInternalAsync()
    {
        // Process all items in the queue (newest SHA wins)
        string? latestSha = null;

        // Drain the queue, keeping only the latest SHA
        while (_pendingShaTasks.TryDequeue(out var sha))
        {
            latestSha = sha;
        }

        if (!string.IsNullOrEmpty(latestSha))
        {
            try
            {
                // Only if not already updating content
                if (!_updateInProgress)
                {
                    _updateInProgress = true;
                    try
                    {
                        Logger.LogInformation("Starting content refresh for SHA {Sha}", latestSha);
                        var startTime = DateTime.UtcNow;

                        // Directly call repositories to avoid semaphore issues in nested calls
                        // Repositories will manage their own semaphores
                        await _contentRepository.RefreshCacheAsync();
                        await _directoryRepository.RefreshCacheAsync();

                        // Update the stored SHA only after successful refresh
                        _lastKnownSha = latestSha;

                        Logger.LogInformation("Content updated successfully to SHA {Sha} in {Duration}ms",
                            latestSha, (DateTime.UtcNow - startTime).TotalMilliseconds);
                    }
                    finally
                    {
                        _updateInProgress = false;
                    }
                }
                else
                {
                    Logger.LogWarning("Update already in progress, skipping update to SHA {Sha}", latestSha);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error refreshing cache for SHA {Sha}", latestSha);
            }
        }
    }

    /// <summary>
    /// Handles GitHub webhook events
    /// </summary>
    public async Task HandleWebhookAsync(string commitSha)
    {
        Logger.LogInformation("Received webhook for commit SHA: {Sha}", commitSha);

        if (string.IsNullOrEmpty(commitSha))
        {
            Logger.LogWarning("Received webhook with empty commit SHA");
            return;
        }

        // Use the same update mechanism as polling
        await UpdateCacheAsync(commitSha, true);
    }
}