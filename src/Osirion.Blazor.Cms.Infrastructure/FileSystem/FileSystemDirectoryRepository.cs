using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Options;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using System.Text;
using System.Text.RegularExpressions;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.FileSystem;

/// <summary>
/// Repository implementation for file system directories
/// </summary>
public class FileSystemDirectoryRepository : RepositoryBase<DirectoryItem, string>, IDirectoryRepository
{
    private readonly FileSystemOptions _options;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private FileSystemWatcher? _fileWatcher;

    // In-memory cache for items
    private Dictionary<string, DirectoryItem>? _directoryCache;
    private DateTime _cacheExpiration = DateTime.MinValue;

    public FileSystemDirectoryRepository(
        IOptions<FileSystemOptions> options,
        ILogger<FileSystemDirectoryRepository> logger)
        : base(GetProviderId(options.Value), logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Setup file watcher if enabled
        if (_options.WatchForChanges)
        {
            SetupFileWatcher();
        }
    }

    private static string GetProviderId(FileSystemOptions options)
    {
        return options.ProviderId ?? $"filesystem-{options.BasePath.GetHashCode():x}";
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<DirectoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_directoryCache == null)
                return new List<DirectoryItem>();

            // Return only root directories (no parent)
            return _directoryCache.Values
                .Where(d => d.Parent == null)
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting all directories");
            throw new ContentProviderException($"Failed to get all directories: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_directoryCache != null && _directoryCache.TryGetValue(id, out var directory))
            {
                return directory;
            }

            return null;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by ID", id);
            throw new ContentProviderException($"Failed to get directory by ID: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            var normalizedPath = NormalizePath(path);
            return _directoryCache?.Values.FirstOrDefault(d => NormalizePath(d.Path) == normalizedPath);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by path", path);
            throw new ContentProviderException($"Failed to get directory by path: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            return _directoryCache?.Values.FirstOrDefault(d => d.Url == url);
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory by URL", url);
            throw new ContentProviderException($"Failed to get directory by URL: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_directoryCache == null)
                return new List<DirectoryItem>();

            if (string.IsNullOrEmpty(locale))
            {
                // Return all root directories
                return _directoryCache.Values
                    .Where(d => d.Parent == null)
                    .ToList();
            }
            else
            {
                // Return directories for the specified locale
                return _directoryCache.Values
                    .Where(d => d.Locale == locale && d.Parent == null)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directories by locale");
            throw new ContentProviderException($"Failed to get directories by locale: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(parentId))
            throw new ArgumentException("Parent ID cannot be empty", nameof(parentId));

        try
        {
            await EnsureCacheIsLoaded(cancellationToken);

            if (_directoryCache == null)
                return new List<DirectoryItem>();

            // Find the parent directory
            if (!_directoryCache.TryGetValue(parentId, out var parent))
                return new List<DirectoryItem>();

            // Return its children
            return parent.Children.ToList();
        }
        catch (Exception ex)
        {
            LogError(ex, "getting children", parentId);
            throw new ContentProviderException($"Failed to get directory children: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get directories by locale (root directories only)
            var rootDirectories = await GetByLocaleAsync(locale, cancellationToken);

            // Children are already populated in the cache
            return rootDirectories;
        }
        catch (Exception ex)
        {
            LogError(ex, "getting directory tree");
            throw new ContentProviderException($"Failed to get directory tree: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            // Check if directory exists
            var existingDirectory = await GetByIdAsync(entity.Id, cancellationToken);
            if (existingDirectory == null)
            {
                // Create new directory
                return await CreateDirectoryAsync(entity, cancellationToken);
            }
            else
            {
                // Update existing directory
                return await UpdateDirectoryAsync(entity, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "saving", entity.Id);
            throw new ContentProviderException($"Failed to save directory: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await DeleteRecursiveAsync(id, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("deleting", id);

        try
        {
            // Get directory to get path
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory == null)
                throw new DirectoryNotFoundException(id);

            // Get the full physical path
            var physicalPath = Path.Combine(_options.BasePath, directory.Path);

            if (!Directory.Exists(physicalPath))
                throw new DirectoryNotFoundException(directory.Path);

            // Delete the directory and all its contents
            Directory.Delete(physicalPath, true);

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);
        }
        catch (DirectoryNotFoundException)
        {
            // Re-throw not found exception
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex, "deleting", id);
            throw new ContentProviderException($"Failed to delete directory: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("moving", id);

        try
        {
            // Get directory to move
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory == null)
                throw new DirectoryNotFoundException(id);

            // Get new parent directory if specified
            DirectoryItem? newParent = null;
            if (!string.IsNullOrEmpty(newParentId))
            {
                newParent = await GetByIdAsync(newParentId, cancellationToken);
                if (newParent == null)
                    throw new DirectoryNotFoundException(newParentId);

                // Check for circular reference
                if (directory.IsAncestorOf(newParent))
                    throw new ContentValidationException("Parent", "Cannot move a directory to one of its descendants");
            }

            // Build physical paths
            var sourcePhysicalPath = Path.Combine(_options.BasePath, directory.Path);

            var newPath = string.IsNullOrEmpty(newParentId)
                ? Path.GetFileName(directory.Path) // Move to root
                : Path.Combine(newParent!.Path, Path.GetFileName(directory.Path));

            var destinationPhysicalPath = Path.Combine(_options.BasePath, newPath);

            // Ensure source exists and destination doesn't
            if (!Directory.Exists(sourcePhysicalPath))
                throw new DirectoryNotFoundException(directory.Path);

            if (Directory.Exists(destinationPhysicalPath))
                throw new ContentValidationException("Path", $"Directory already exists at destination: {newPath}");

            // Create destination directory structure if needed
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPhysicalPath)!);

            // Move the directory
            Directory.Move(sourcePhysicalPath, destinationPhysicalPath);

            // Update directory entity
            var updatedDirectory = directory.Clone();
            updatedDirectory.SetParent(newParent);
            updatedDirectory.SetPath(newPath);

            // Save updated directory metadata
            await SaveDirectoryMetadataAsync(updatedDirectory, cancellationToken);

            // Refresh cache
            await RefreshCacheAsync(cancellationToken);

            return updatedDirectory;
        }
        catch (Exception ex)
        {
            LogError(ex, "moving", id);
            throw new ContentProviderException($"Failed to move directory: {ex.Message}", ex, ProviderId);
        }
    }

    /// <inheritdoc/>
    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            _directoryCache = null;
            _cacheExpiration = DateTime.MinValue;

            // Force reload
            await EnsureCacheIsLoaded(cancellationToken, forceRefresh: true);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    #region Helper Methods

    private async Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
    {
        if (!forceRefresh && _directoryCache != null && DateTime.UtcNow < _cacheExpiration)
        {
            return; // Cache is still valid
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check inside the lock
            if (!forceRefresh && _directoryCache != null && DateTime.UtcNow < _cacheExpiration)
            {
                return; // Cache was populated while waiting for lock
            }

            // Load all directories
            var cache = new Dictionary<string, DirectoryItem>();

            // Ensure base path exists
            if (!Directory.Exists(_options.BasePath))
            {
                if (_options.CreateDirectoriesIfNotExist)
                {
                    Directory.CreateDirectory(_options.BasePath);
                }
                else
                {
                    throw new DirectoryNotFoundException($"Base directory does not exist: {_options.BasePath}");
                }
            }

            // Scan directories
            await ScanDirectoriesAsync(_options.BasePath, cache, null, cancellationToken);

            // Update cache
            _directoryCache = cache;
            _cacheExpiration = DateTime.UtcNow.AddMinutes(_options.CacheDurationMinutes);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task ScanDirectoriesAsync(
        string directoryPath,
        Dictionary<string, DirectoryItem> cache,
        DirectoryItem? parentDirectory,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            if (!dirInfo.Exists)
                return;

            // Skip directories that match exclude patterns
            if (_options.ExcludePatterns.Any(pattern => MatchesGlobPattern(dirInfo.FullName, pattern)))
                return;

            // Get the relative path from the base path
            var relativePath = Path.GetRelativePath(_options.BasePath, directoryPath);
            if (relativePath == ".")
                relativePath = "";

            // Create directory item for this directory
            var directoryId = relativePath.GetHashCode().ToString("x");
            var directory = DirectoryItem.Create(
                directoryId,
                NormalizePath(relativePath),
                dirInfo.Name,
                ProviderId);

            // Set parent if available
            if (parentDirectory != null)
            {
                directory.SetParent(parentDirectory);
                parentDirectory.AddChild(directory);
            }

            // Extract locale from path if enabled
            if (_options.EnableLocalization)
            {
                directory.SetLocale(ExtractLocaleFromPath(relativePath));
            }
            else
            {
                directory.SetLocale(_options.DefaultLocale);
            }

            // Add metadata from _index.md if exists
            await ProcessDirectoryMetadataAsync(directory, directoryPath, cancellationToken);

            // Add to cache
            cache[directoryId] = directory;

            // Process subdirectories
            if (_options.IncludeSubdirectories)
            {
                foreach (var subdirInfo in dirInfo.GetDirectories())
                {
                    // Skip directories that match exclude patterns
                    if (_options.ExcludePatterns.Any(pattern => MatchesGlobPattern(subdirInfo.FullName, pattern)))
                        continue;

                    await ScanDirectoriesAsync(
                        subdirInfo.FullName,
                        cache,
                        directory,
                        cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error scanning directory: {DirectoryPath}", directoryPath);
        }
    }

    private async Task ProcessDirectoryMetadataAsync(DirectoryItem directory, string directoryPath, CancellationToken cancellationToken)
    {
        var indexPath = Path.Combine(directoryPath, "_index.md");
        if (File.Exists(indexPath))
        {
            try
            {
                var content = await File.ReadAllTextAsync(indexPath, cancellationToken);
                var frontMatterEndIndex = content.IndexOf("---", 4);
                if (frontMatterEndIndex > 0)
                {
                    var frontMatter = content.Substring(4, frontMatterEndIndex - 4).Trim();
                    var metadata = ParseDirectoryFrontMatter(frontMatter);

                    // Apply metadata to directory
                    if (metadata.TryGetValue("title", out var title))
                        directory.SetName(title);

                    if (metadata.TryGetValue("description", out var description))
                        directory.SetDescription(description);

                    if (metadata.TryGetValue("order", out var orderStr) && int.TryParse(orderStr, out var order))
                        directory.SetOrder(order);

                    if (metadata.TryGetValue("url", out var url))
                        directory.SetUrl(url);

                    // Add other metadata as properties
                    foreach (var kvp in metadata.Where(k => k.Key != "title" && k.Key != "description" && k.Key != "order" && k.Key != "url"))
                    {
                        directory.SetMetadata(kvp.Key, kvp.Value);
                    }

                    // Store physical path as provider-specific ID
                    directory.SetProviderSpecificId(indexPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing directory metadata: {IndexPath}", indexPath);
            }
        }
    }

    private async Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem entity, CancellationToken cancellationToken)
    {
        LogOperation("creating", entity.Id);

        // Build the full physical path
        var physicalPath = Path.Combine(_options.BasePath, entity.Path);

        // Create the directory if it doesn't exist
        if (!Directory.Exists(physicalPath))
        {
            Directory.CreateDirectory(physicalPath);
        }

        // Save metadata
        await SaveDirectoryMetadataAsync(entity, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return entity;
    }

    private async Task<DirectoryItem> UpdateDirectoryAsync(DirectoryItem entity, CancellationToken cancellationToken)
    {
        LogOperation("updating", entity.Id);

        // Save metadata
        await SaveDirectoryMetadataAsync(entity, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return entity;
    }

    private async Task SaveDirectoryMetadataAsync(DirectoryItem directory, CancellationToken cancellationToken)
    {
        var physicalPath = Path.Combine(_options.BasePath, directory.Path);
        var indexPath = Path.Combine(physicalPath, "_index.md");

        var frontMatter = new StringBuilder();
        frontMatter.AppendLine("---");

        // Add metadata
        frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

        if (!string.IsNullOrEmpty(directory.Description))
            frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

        if (directory.Order != 0)
            frontMatter.AppendLine($"order: {directory.Order}");

        if (!string.IsNullOrEmpty(directory.Url))
            frontMatter.AppendLine($"url: \"{directory.Url}\"");

        if (!string.IsNullOrEmpty(directory.Locale))
            frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

        // Add custom metadata
        foreach (var kvp in directory.Metadata)
        {
            if (kvp.Value is string strValue)
                frontMatter.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (kvp.Value is bool boolValue)
                frontMatter.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (kvp.Value is int intValue)
                frontMatter.AppendLine($"{kvp.Key}: {intValue}");
            else if (kvp.Value is double doubleValue)
                frontMatter.AppendLine($"{kvp.Key}: {doubleValue}");
            else
                frontMatter.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        }

        frontMatter.AppendLine("---");
        frontMatter.AppendLine();
        frontMatter.AppendLine($"# {directory.Name}");
        frontMatter.AppendLine();

        if (!string.IsNullOrEmpty(directory.Description))
        {
            frontMatter.AppendLine(directory.Description);
        }

        // Ensure directory exists
        Directory.CreateDirectory(physicalPath);

        // Write the _index.md file
        await File.WriteAllTextAsync(indexPath, frontMatter.ToString(), cancellationToken);

        // Store the file path as provider-specific ID
        directory.SetProviderSpecificId(indexPath);
    }

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    private Dictionary<string, string> ParseDirectoryFrontMatter(string frontMatter)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var lines = frontMatter.Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Split(':', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim().ToLowerInvariant();
            var value = parts[1].Trim();

            // Remove quotes if present
            if (value.StartsWith("\"") && value.EndsWith("\"") ||
                value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }

            result[key] = value;
        }

        return result;
    }

    private string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private bool MatchesGlobPattern(string path, string pattern)
    {
        // Convert glob pattern to regex
        var regexPattern = pattern
            .Replace(".", "\\.")
            .Replace("*", ".*")
            .Replace("?", ".")
            .Replace("\\*\\*", ".*");

        return Regex.IsMatch(path, $"^{regexPattern}$", RegexOptions.IgnoreCase);
    }

    private string ExtractLocaleFromPath(string path)
    {
        // If localization is disabled, always return default locale
        if (!_options.EnableLocalization)
        {
            return _options.DefaultLocale;
        }

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && _options.SupportedLocales.Contains(segments[0]))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return _options.DefaultLocale;
    }

    private void SetupFileWatcher()
    {
        try
        {
            if (!Directory.Exists(_options.BasePath))
            {
                if (_options.CreateDirectoriesIfNotExist)
                {
                    Directory.CreateDirectory(_options.BasePath);
                }
                else
                {
                    Logger.LogWarning("Cannot set up file watcher - base path does not exist: {BasePath}", _options.BasePath);
                    return;
                }
            }

            _fileWatcher = new FileSystemWatcher(_options.BasePath)
            {
                IncludeSubdirectories = _options.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite /*| NotifyFilters.Created | NotifyFilters.Deleted*/,
                EnableRaisingEvents = true
            };

            // Add handler for all change events
            _fileWatcher.Changed += OnDirectoryChanged;
            _fileWatcher.Created += OnDirectoryChanged;
            _fileWatcher.Deleted += OnDirectoryChanged;
            _fileWatcher.Renamed += OnDirectoryChanged;

            Logger.LogInformation("Directory watcher set up for path: {BasePath}", _options.BasePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set up directory watcher for {BasePath}", _options.BasePath);
        }
    }

    private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
    {
        if (Directory.Exists(e.FullPath) ||
            Path.GetFileName(e.FullPath) == "_index.md" ||
            e.ChangeType == WatcherChangeTypes.Deleted)
        {
            Logger.LogInformation("Directory change detected: {ChangeType} - {Path}", e.ChangeType, e.FullPath);

            // Invalidate cache asynchronously
            Task.Run(() => RefreshCacheAsync());
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileWatcher?.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}