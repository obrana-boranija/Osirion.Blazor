using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;
using Osirion.Blazor.Cms.Infrastructure.Options;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using System.Text;
using System.Text.RegularExpressions;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Repository implementation for GitHub directories
/// </summary>
public class GitHubDirectoryRepository : RepositoryBase<DirectoryItem, string>, IDirectoryRepository
{
    private readonly IGitHubApiClient _apiClient;
    private readonly GitHubOptions _options;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    // In-memory cache for directories
    private Dictionary<string, DirectoryItem>? _directoryCache;
    private DateTime _cacheExpiration = DateTime.MinValue;

    public GitHubDirectoryRepository(
        IGitHubApiClient apiClient,
        IOptions<GitHubOptions> options,
        ILogger<GitHubDirectoryRepository> logger)
        : base(GetProviderId(options.Value), logger)
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
                // Return all directories grouped by locale
                return _directoryCache.Values
                    .Where(d => d.Parent == null) // Get only root directories
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
    public async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("deleting", id);

        try
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
            var contentPath = NormalizePath(_options.ContentPath);

            // Get repository contents
            var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, cancellationToken);

            // Process contents recursively to build directory structure
            await ProcessDirectoriesRecursivelyAsync(contents, cache, null, cancellationToken);

            // Process _index.md files for metadata
            await ProcessDirectoryMetadataAsync(cache, cancellationToken);

            // Update cache
            _directoryCache = cache;
            _cacheExpiration = DateTime.UtcNow.AddMinutes(_options.CacheDurationMinutes);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task ProcessDirectoriesRecursivelyAsync(
        List<GitHubItem> contents,
        Dictionary<string, DirectoryItem> directoryCache,
        DirectoryItem? parentDirectory,
        CancellationToken cancellationToken)
    {
        foreach (var item in contents.Where(i => i.IsDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Create directory item
            var directoryId = item.Path.GetHashCode().ToString("x");
            var directory = DirectoryItem.Create(
                id: directoryId,
                path: item.Path,
                name: item.Name,
                providerId: ProviderId);

            // Set parent if available
            if (parentDirectory != null)
            {
                directory.SetParent(parentDirectory);
                parentDirectory.AddChild(directory);
            }

            // Extract locale from path if enabled
            if (_options.EnableLocalization)
            {
                directory.SetLocale(ExtractLocaleFromPath(item.Path));
            }
            else
            {
                directory.SetLocale(_options.DefaultLocale);
            }

            // Add to cache
            directoryCache[directoryId] = directory;

            // Process subdirectories
            var subContents = await _apiClient.GetRepositoryContentsAsync(item.Path, cancellationToken);
            await ProcessDirectoriesRecursivelyAsync(subContents, directoryCache, directory, cancellationToken);
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

                // Extract front matter
                var frontMatter = ExtractFrontMatter(decodedContent);

                // Find the directory
                var directory = directoryCache.Values.FirstOrDefault(d => d.Path == directoryPath);
                if (directory != null)
                {
                    // Update directory with metadata from _index.md
                    ApplyFrontMatterToDirectory(directory, frontMatter);

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

    private Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Look for front matter between --- delimiters
        var match = Regex.Match(content, @"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Remove quotes if present
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    private void ApplyFrontMatterToDirectory(DirectoryItem directory, Dictionary<string, string> frontMatter)
    {
        foreach (var kvp in frontMatter)
        {
            var key = kvp.Key.ToLowerInvariant();
            var value = kvp.Value;

            switch (key)
            {
                case "title":
                    directory.SetName(value);
                    break;
                case "description":
                    directory.SetDescription(value);
                    break;
                case "order":
                    if (int.TryParse(value, out var order))
                        directory.SetOrder(order);
                    break;
                case "locale":
                    directory.SetLocale(value);
                    break;
                case "url":
                    directory.SetUrl(value);
                    break;
                default:
                    // Add as custom metadata
                    if (bool.TryParse(value, out var boolVal))
                        directory.SetMetadata(key, boolVal);
                    else if (int.TryParse(value, out var intVal))
                        directory.SetMetadata(key, intVal);
                    else if (double.TryParse(value, out var doubleVal))
                        directory.SetMetadata(key, doubleVal);
                    else
                        directory.SetMetadata(key, value);
                    break;
            }
        }

        // If URL is not set, generate a default one
        if (string.IsNullOrEmpty(directory.Url))
        {
            directory.SetUrl(GenerateDirectoryUrl(directory.Path));
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
        var frontMatter = new StringBuilder();
        frontMatter.AppendLine("---");

        // Add metadata
        frontMatter.AppendLine($"title: \"{EscapeYamlString(directory.Name)}\"");

        if (!string.IsNullOrEmpty(directory.Description))
            frontMatter.AppendLine($"description: \"{EscapeYamlString(directory.Description)}\"");

        if (directory.Order != 0)
            frontMatter.AppendLine($"order: {directory.Order}");

        if (!string.IsNullOrEmpty(directory.Locale))
            frontMatter.AppendLine($"locale: \"{directory.Locale}\"");

        if (!string.IsNullOrEmpty(directory.Url))
            frontMatter.AppendLine($"url: \"{directory.Url}\"");

        // Add custom metadata
        foreach (var meta in directory.Metadata)
        {
            if (meta.Value is string strValue)
                frontMatter.AppendLine($"{meta.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (meta.Value is bool boolValue)
                frontMatter.AppendLine($"{meta.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (meta.Value is int intValue)
                frontMatter.AppendLine($"{meta.Key}: {intValue}");
            else if (meta.Value is double doubleValue)
                frontMatter.AppendLine($"{meta.Key}: {doubleValue}");
            else
                frontMatter.AppendLine($"{meta.Key}: \"{meta.Value}\"");
        }

        frontMatter.AppendLine("---");

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
            frontMatter.ToString(),
            commitMessage,
            sha,
            cancellationToken);

        // Update the entity with the new SHA
        directory.SetProviderSpecificId(response.Content.Sha);
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

    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    private string ExtractLocaleFromPath(string path)
    {
        // If localization is disabled, always return default locale
        if (!_options.EnableLocalization)
        {
            return _options.DefaultLocale;
        }

        // Check if content path is set and remove it from the beginning
        var contentPath = NormalizePath(_options.ContentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0]))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return _options.DefaultLocale;
    }

    private bool IsValidLocale(string locale)
    {
        // Check against supported locales list if defined
        if (_options.SupportedLocales.Count > 0)
        {
            return _options.SupportedLocales.Contains(locale, StringComparer.OrdinalIgnoreCase);
        }

        // Fallback to simple validation: 2-letter language code or language-region format
        return (locale.Length == 2 && locale.All(char.IsLetter)) ||
               (locale.Length == 5 && locale[2] == '-' &&
                locale.Substring(0, 2).All(char.IsLetter) &&
                locale.Substring(3, 2).All(char.IsLetter));
    }

    private string GenerateDirectoryUrl(string path)
    {
        // Normalize path
        path = NormalizePath(path);

        // Remove content path prefix if present
        var contentPath = NormalizePath(_options.ContentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // If using localization, check if the first segment is a locale and remove it
        if (_options.EnableLocalization && !string.IsNullOrEmpty(path))
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0]))
            {
                path = string.Join("/", segments.Skip(1));
            }
        }

        return path;
    }

    #endregion
}