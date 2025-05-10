using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Repository implementation for GitHub directories
/// </summary>
public class GitHubDirectoryRepository : DirectoryRepositoryBase, IDirectoryRepository
{
    private readonly IGitHubApiClient _apiClient;
    private readonly GitHubOptions _options;

    public GitHubDirectoryRepository(
        IGitHubApiClient apiClient,
        IOptions<GitHubOptions> options,
        IDirectoryCacheManager cacheManager,
        IDirectoryMetadataProcessor metadataProcessor,
        IPathUtilities pathUtils,
        ILogger<GitHubDirectoryRepository> logger)
        : base(GetProviderId(options.Value), cacheManager, metadataProcessor, pathUtils, options, logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Configure API client
        _apiClient.SetRepository(_options.Owner, _options.Repository);
        _apiClient.SetBranch(_options.Branch);

        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            _apiClient.SetAccessToken(_options.ApiToken);
        }
    }

    private static string GetProviderId(GitHubOptions options)
    {
        return options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";
    }

    /// <inheritdoc/>
    public override async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Use default commit message
        var commitMessage = string.IsNullOrEmpty(entity.ProviderSpecificId)
            ? $"Create directory {entity.Name}"
            : $"Update directory {entity.Name}";

        try
        {
            // Check if directory exists
            var existingDirectory = await GetByIdAsync(entity.Id, cancellationToken);
            if (existingDirectory == null)
            {
                // Create new directory
                return await CreateDirectoryAsync(entity, commitMessage, cancellationToken);
            }
            else
            {
                // Update existing directory
                return await UpdateDirectoryAsync(entity, commitMessage, cancellationToken);
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

            // Determine old and new paths
            var oldPath = directory.Path;
            var newPath = string.IsNullOrEmpty(newParentId)
                ? Path.GetFileName(directory.Path) // Move to root
                : Path.Combine(newParent!.Path, Path.GetFileName(directory.Path)).Replace('\\', '/');

            // Move all files and directories
            await MoveDirectoryContentsAsync(oldPath, newPath, cancellationToken);

            // Update directory entity
            var updatedDirectory = directory.Clone();
            updatedDirectory.SetParent(newParent);
            updatedDirectory.SetPath(newPath);

            // Save updated directory metadata
            await SaveAsync(updatedDirectory, cancellationToken);

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
        var contentPath = PathUtils.NormalizePath(_options.ContentPath);

        // Get repository contents
        var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, cancellationToken);

        // Process contents recursively to build directory structure
        await ProcessDirectoriesRecursivelyAsync(contents, cache, null, cancellationToken);

        // Process _index.md files for metadata
        await ProcessDirectoryMetadataAsync(cache, cancellationToken);

        return cache;
    }

    /// <inheritdoc/>
    protected override async Task DeleteDirectoryInternalAsync(
        string id,
        bool recursive,
        string? commitMessage,
        CancellationToken cancellationToken)
    {
        // Get directory to get path and metadata
        var directory = await GetByIdAsync(id, cancellationToken);
        if (directory == null)
            throw new DirectoryNotFoundException(id);

        // Generate a default commit message if not provided
        var message = commitMessage ?? $"Delete directory {directory.Name}";

        // Get all files in the directory and its subdirectories
        var allFiles = await GetAllFilesInPathAsync(directory.Path, cancellationToken);

        // Delete all files in reverse order (deeper paths first)
        foreach (var file in allFiles.OrderByDescending(f => f.Path.Count(c => c == '/')))
        {
            await _apiClient.DeleteFileAsync(
                file.Path,
                message,
                file.Sha,
                cancellationToken);
        }
    }

    /// <summary>
    /// Process directories recursively with additional safety checks
    /// </summary>
    private async Task ProcessDirectoriesRecursivelyAsync(
        List<GitHubItem> contents,
        Dictionary<string, DirectoryItem> directoryCache,
        DirectoryItem? parentDirectory,
        CancellationToken cancellationToken,
        HashSet<string>? processedPaths = null)
    {
        // Initialize path tracking if not provided
        processedPaths ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in contents.Where(i => i.IsDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Skip if we've already processed this path to avoid cycles
            if (!processedPaths.Add(item.Path))
            {
                Logger.LogWarning("Skipping already processed directory path: {Path}", item.Path);
                continue;
            }

            // Create directory item with more robust ID generation
            var directoryId = IdGenerator.GenerateDirectoryId(item.Path, item.Name, GetProviderId(_options));

            // Check if we already have this directory in cache
            if (directoryCache.TryGetValue(directoryId, out var existingDirectory))
            {
                // If this directory already exists with a different parent, we have a problem
                if (parentDirectory != null && existingDirectory.Parent != null &&
                    existingDirectory.Parent.Id != parentDirectory.Id)
                {
                    Logger.LogWarning(
                        "Directory '{Name}' ({Id}) already exists with different parent. Path: {Path}",
                        item.Name, directoryId, item.Path);

                    // Skip this to avoid circular references
                    continue;
                }

                // Use existing directory
                var directory = existingDirectory;

                // Update parent if needed
                if (parentDirectory != null && directory.Parent == null)
                {
                    try
                    {
                        parentDirectory.AddChild(directory);
                    }
                    catch (ContentValidationException ex)
                    {
                        Logger.LogWarning(ex,
                            "Cannot set parent-child relationship between '{Parent}' and '{Child}'",
                            parentDirectory.Name, directory.Name);
                        // Continue processing other directories
                    }
                }
            }
            else
            {
                // Create new directory
                var directory = DirectoryItem.Create(
                    id: directoryId,
                    path: item.Path,
                    name: item.Name,
                    providerId: ProviderId);

                // Set parent if available
                if (parentDirectory != null)
                {
                    try
                    {
                        parentDirectory.AddChild(directory);
                    }
                    catch (ContentValidationException ex)
                    {
                        Logger.LogWarning(ex,
                            "Cannot set parent-child relationship between '{Parent}' and '{Child}'",
                            parentDirectory.Name, directory.Name);
                        // Still add to cache but without parent relationship
                    }
                }

                // Extract locale from path if enabled
                if (_options.EnableLocalization)
                {
                    directory.SetLocale(PathUtils.ExtractLocaleFromPath(item.Path));
                }
                else
                {
                    directory.SetLocale(_options.DefaultLocale);
                }

                // Add to cache
                directoryCache[directoryId] = directory;
            }

            // Get an updated reference to the directory from cache
            var dirForProcessing = directoryCache[directoryId];

            // Process subdirectories with safety tracking
            var subContents = await _apiClient.GetRepositoryContentsAsync(item.Path, cancellationToken);
            await ProcessDirectoriesRecursivelyAsync(
                subContents,
                directoryCache,
                dirForProcessing,
                cancellationToken,
                processedPaths);
        }
    }

    private async Task ProcessDirectoryMetadataAsync(
        Dictionary<string, DirectoryItem> directoryCache,
        CancellationToken cancellationToken)
    {
        // Process _index.md files for each directory
        foreach (var directoryPath in directoryCache.Values.Select(d => d.Path))
        {
            try
            {
                var indexFilePath = Path.Combine(directoryPath, "_index.md").Replace('\\', '/');
                var fileContent = await _apiClient.GetFileContentAsync(indexFilePath, cancellationToken);
                var decodedContent = fileContent.GetDecodedContent();

                // Find the directory
                var directory = directoryCache.Values.FirstOrDefault(d => d.Path == directoryPath);
                if (directory != null)
                {
                    // Process metadata
                    MetadataProcessor.ProcessMetadata(directory, decodedContent);

                    // Save SHA for updates
                    directory.SetProviderSpecificId(fileContent.Sha);
                }
            }
            catch (Exception)
            {
                // _index.md might not exist - that's ok
                continue;
            }
        }
    }

    private async Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem entity, string commitMessage, CancellationToken cancellationToken)
    {
        LogOperation("creating", entity.Id);

        // GitHub doesn't support empty directories, so we create a placeholder file
        var placeholderPath = Path.Combine(entity.Path, ".gitkeep").Replace('\\', '/');

        // Create placeholder file
        await _apiClient.CreateOrUpdateFileAsync(
            placeholderPath,
            "", // Empty content
            commitMessage,
            null,
            cancellationToken);

        // Create _index.md with directory metadata
        await SaveDirectoryMetadataAsync(entity, commitMessage, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return entity;
    }

    private async Task<DirectoryItem> UpdateDirectoryAsync(DirectoryItem entity, string commitMessage, CancellationToken cancellationToken)
    {
        LogOperation("updating", entity.Id);

        // Update _index.md with directory metadata
        await SaveDirectoryMetadataAsync(entity, commitMessage, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return entity;
    }

    private async Task SaveDirectoryMetadataAsync(DirectoryItem directory, string commitMessage, CancellationToken cancellationToken)
    {
        var indexPath = Path.Combine(directory.Path, "_index.md").Replace('\\', '/');

        // Generate metadata content
        var content = MetadataProcessor.GenerateMetadataContent(directory);

        // Check if _index.md already exists
        string? sha = directory.ProviderSpecificId;
        if (string.IsNullOrEmpty(sha))
        {
            try
            {
                var existingFile = await _apiClient.GetFileContentAsync(indexPath, cancellationToken);
                sha = existingFile.Sha;
            }
            catch
            {
                // File doesn't exist yet
            }
        }

        // Create or update _index.md
        var response = await _apiClient.CreateOrUpdateFileAsync(
            indexPath,
            content,
            commitMessage,
            sha,
            cancellationToken);

        // Update the entity with the new SHA
        directory.SetProviderSpecificId(response.Content.Sha);
    }

    private async Task<List<GitHubItem>> GetAllFilesInPathAsync(string path, CancellationToken cancellationToken)
    {
        var result = new List<GitHubItem>();
        var contents = await _apiClient.GetRepositoryContentsAsync(path, cancellationToken);

        foreach (var item in contents)
        {
            if (item.IsFile)
            {
                result.Add(item);
            }
            else if (item.IsDirectory)
            {
                // Recursively get files in subdirectory
                var subFiles = await GetAllFilesInPathAsync(item.Path, cancellationToken);
                result.AddRange(subFiles);
            }
        }

        return result;
    }

    private async Task MoveDirectoryContentsAsync(string oldPath, string newPath, CancellationToken cancellationToken)
    {
        // Get all files in the source directory
        var files = await GetAllFilesInPathAsync(oldPath, cancellationToken);

        // Commit message for the operations
        var commitMessage = $"Move directory from {oldPath} to {newPath}";

        // Create files at the new location
        foreach (var file in files)
        {
            // Calculate new file path
            var relativePath = file.Path.Substring(oldPath.Length);
            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            var newFilePath = Path.Combine(newPath, relativePath).Replace('\\', '/');

            // Get file content
            var fileContent = await _apiClient.GetFileContentAsync(file.Path, cancellationToken);
            var content = fileContent.GetDecodedContent();

            // Create file at new location
            await _apiClient.CreateOrUpdateFileAsync(
                newFilePath,
                content,
                commitMessage,
                null, // No SHA because it's a new file at this path
                cancellationToken);

            // Delete original file
            await _apiClient.DeleteFileAsync(
                file.Path,
                commitMessage,
                file.Sha,
                cancellationToken);
        }
    }
}