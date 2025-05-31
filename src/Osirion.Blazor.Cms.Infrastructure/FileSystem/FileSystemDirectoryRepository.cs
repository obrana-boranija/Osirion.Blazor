using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.FileSystem;

/// <summary>
/// Repository implementation for file system directories
/// </summary>
public class FileSystemDirectoryRepository : DirectoryRepositoryBase, IDirectoryRepository
{
    private readonly FileSystemOptions _options;
    private FileSystemWatcher? _fileWatcher;

    public FileSystemDirectoryRepository(
        IOptions<FileSystemOptions> options,
        IDirectoryCacheManager cacheManager,
        IDirectoryMetadataProcessor metadataProcessor,
        IPathUtilities pathUtils,
        ILogger<FileSystemDirectoryRepository> logger)
        : base(GetProviderId(options.Value), cacheManager, metadataProcessor, pathUtils, options, logger)
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
    public override async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            // Check if directory exists
            var existingDirectory = await GetByIdAsync(entity.Id, cancellationToken);
            if (existingDirectory is null)
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
    public async Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("moving", id);

        try
        {
            // Get directory to move
            var directory = await GetByIdAsync(id, cancellationToken);
            if (directory is null)
                throw new DirectoryNotFoundException(id);

            // Get new parent directory if specified
            DirectoryItem? newParent = null;
            if (!string.IsNullOrWhiteSpace(newParentId))
            {
                newParent = await GetByIdAsync(newParentId, cancellationToken);
                if (newParent is null)
                    throw new DirectoryNotFoundException(newParentId);

                // Check for circular reference
                if (directory.IsAncestorOf(newParent))
                    throw new ContentValidationException("Parent", "Cannot move a directory to one of its descendants");
            }

            // Build physical paths
            var sourcePhysicalPath = Path.Combine(_options.BasePath, directory.Path);

            var newPath = string.IsNullOrWhiteSpace(newParentId)
                ? Path.GetFileName(directory.Path) // Move to root
                : Path.Combine(newParent!.Path, Path.GetFileName(directory.Path));

            var destinationPhysicalPath = Path.Combine(_options.BasePath, newPath);

            if (!System.IO.Directory.Exists(sourcePhysicalPath))
                throw new DirectoryNotFoundException(directory.Path);

            if (!System.IO.Directory.Exists(sourcePhysicalPath))
                throw new ContentValidationException("Path", $"Directory already exists at destination: {newPath}");

            // Create destination directory structure if needed
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(destinationPhysicalPath)!);

            // Move the directory
            System.IO.Directory.Move(sourcePhysicalPath, destinationPhysicalPath);

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
    protected override async Task<Dictionary<string, DirectoryItem>> LoadDirectoriesAsync(
        CancellationToken cancellationToken)
    {
        var cache = new Dictionary<string, DirectoryItem>();

        // Ensure base path exists
        if (!System.IO.Directory.Exists(_options.BasePath))
        {
            if (_options.CreateDirectoriesIfNotExist)
            {
                System.IO.Directory.CreateDirectory(_options.BasePath);
            }
            else
            {
                throw new DirectoryNotFoundException($"Base directory does not exist: {_options.BasePath}");
            }
        }

        // Scan directories
        await ScanDirectoriesAsync(_options.BasePath, cache, null, cancellationToken);

        return cache;
    }

    /// <inheritdoc/>
    protected override async Task DeleteDirectoryInternalAsync(
        string id,
        bool recursive,
        string? commitMessage,
        CancellationToken cancellationToken)
    {
        // Get directory to get path
        var directory = await GetByIdAsync(id, cancellationToken);
        if (directory is null)
            throw new DirectoryNotFoundException(id);

        // Get the full physical path
        var physicalPath = Path.Combine(_options.BasePath, directory.Path);

        if (!System.IO.Directory.Exists(physicalPath))
            throw new DirectoryNotFoundException(directory.Path);

        // Delete the directory and all its contents
        System.IO.Directory.Delete(physicalPath, recursive);
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
            if (_options.ExcludePatterns.Any(pattern =>
                PathUtils.MatchesGlobPattern(dirInfo.FullName, pattern)))
                return;

            // Get the relative path from the base path
            var relativePath = Path.GetRelativePath(_options.BasePath, directoryPath).Replace('\\', '/');
            if (relativePath == ".")
                relativePath = "";

            // Create directory item for this directory
            var directoryId = relativePath.GetHashCode().ToString("x");
            var directory = DirectoryItem.Create(
                directoryId,
                PathUtils.NormalizePath(relativePath),
                dirInfo.Name,
                ProviderId);

            // Set parent if available
            if (parentDirectory is not null)
            {
                directory.SetParent(parentDirectory);
                parentDirectory.AddChild(directory);
            }

            // Extract locale from path if enabled
            if (_options.EnableLocalization)
            {
                directory.SetLocale(PathUtils.ExtractLocaleFromPath(relativePath));
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
                    if (_options.ExcludePatterns.Any(pattern =>
                        PathUtils.MatchesGlobPattern(subdirInfo.FullName, pattern)))
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

    private async Task ProcessDirectoryMetadataAsync(
        DirectoryItem directory,
        string directoryPath,
        CancellationToken cancellationToken)
    {
        var indexPath = Path.Combine(directoryPath, "_index.md");
        if (File.Exists(indexPath))
        {
            try
            {
                var content = await File.ReadAllTextAsync(indexPath, cancellationToken);

                // Process metadata
                MetadataProcessor.ProcessMetadata(directory, content);

                // Store physical path as provider-specific ID
                directory.SetProviderSpecificId(indexPath);
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
        if (!System.IO.Directory.Exists(physicalPath))
        {
            System.IO.Directory.CreateDirectory(physicalPath);
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

        // Generate metadata content
        var content = MetadataProcessor.GenerateMetadataContent(directory);

        // Ensure directory exists
        System.IO.Directory.CreateDirectory(physicalPath);

        // Write the _index.md file
        await File.WriteAllTextAsync(indexPath, content, cancellationToken);

        // Store the file path as provider-specific ID
        directory.SetProviderSpecificId(indexPath);
    }

    private void SetupFileWatcher()
    {
        try
        {
            if (!System.IO.Directory.Exists(_options.BasePath))
            {
                if (_options.CreateDirectoriesIfNotExist)
                {
                    System.IO.Directory.CreateDirectory(_options.BasePath);
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
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
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
        if (System.IO.Directory.Exists(e.FullPath) ||
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
}