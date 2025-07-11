﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using System.IO.Abstractions;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.FileSystem
{
    /// <summary>
    /// Repository implementation for file system content
    /// </summary>
    public class FileSystemContentRepository : BaseContentRepository
    {
        private readonly FileSystemOptions _options;
        private readonly Dictionary<string, string> _fileExtensionContentTypeMap;
        private readonly IFileSystem _fileSystem;
        private FileSystemWatcher? _fileWatcher;

        public FileSystemContentRepository(
            IMarkdownProcessor markdownProcessor,
            IOptions<FileSystemOptions> options,
            ILogger<FileSystemContentRepository> logger,
            IFileSystem fileSystem)
            : base(GetProviderId(options.Value), markdownProcessor, logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _fileSystem = fileSystem;

            // Set base class properties
            EnableLocalization = _options.EnableLocalization;
            DefaultLocale = _options.DefaultLocale;
            SupportedLocales = _options.SupportedLocales;
            ContentPath = _options.ContentRoot ?? string.Empty;

            // Initialize file extension to content type mapping
            _fileExtensionContentTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".md", "text/markdown" },
                { ".markdown", "text/markdown" },
                { ".txt", "text/plain" }
            };

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
        protected async Task EnsureCacheIsLoaded(CancellationToken cancellationToken, bool forceRefresh = false)
        {
            if (!forceRefresh && ItemCache is not null)
            {
                return; // Cache is still valid
            }

            await CacheLock.WaitAsync(cancellationToken);
            try
            {
                // Double-check inside the lock
                if (!forceRefresh && ItemCache is not null)
                {
                    return; // Cache was populated while waiting for lock
                }

                // Load all content items
                var cache = new Dictionary<string, ContentItem>();

                // Create base directory if it doesn't exist
                if (!_fileSystem.Directory.Exists(_options.BasePath))
                {
                    if (_options.CreateDirectoriesIfNotExist)
                    {
                        _fileSystem.Directory.CreateDirectory(_options.BasePath);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Base content directory does not exist: {_options.BasePath}");
                    }
                }

                // Find all markdown files
                var files = new List<string>();
                foreach (var extension in _options.SupportedExtensions)
                {
                    files.AddRange(_fileSystem.Directory.GetFiles(
                        _options.BasePath,
                        $"*{extension}",
                        _options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                }

                // Process each file
                foreach (var file in files.Where(f => !IsExcluded(f)))
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var contentItem = await ProcessMarkdownFileAsync(file, cancellationToken);
                        if (contentItem is not null)
                        {
                            cache[contentItem.Id] = contentItem;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error processing file: {FileName}", file);
                    }
                }

                // Update cache
                ItemCache = cache;
            }
            finally
            {
                CacheLock.Release();
            }
        }

        /// <inheritdoc/>
        protected override async Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken)
        {
            var cache = new Dictionary<string, ContentItem>();

            try
            {
                // Create base directory if it doesn't exist
                if (!_fileSystem.Directory.Exists(_options.BasePath))
                {
                    if (_options.CreateDirectoriesIfNotExist)
                    {
                        _fileSystem.Directory.CreateDirectory(_options.BasePath);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Base content directory does not exist: {_options.BasePath}");
                    }
                }

                // Find all markdown files
                var files = new List<string>();
                foreach (var extension in _options.SupportedExtensions)
                {
                    files.AddRange(_fileSystem.Directory.GetFiles(
                        _options.BasePath,
                        $"*{extension}",
                        _options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
                }

                // Process each file
                foreach (var file in files.Where(f => !IsExcluded(f)))
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var contentItem = await ProcessMarkdownFileAsync(file, cancellationToken);
                        if (contentItem is not null)
                        {
                            cache[contentItem.Id] = contentItem;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error processing file: {FileName}", file);
                    }
                }

                Logger.LogInformation("Loaded {Count} content items from file system at {BasePath}",
                    cache.Count, _options.BasePath);

                return cache;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading content from file system at {BasePath}", _options.BasePath);
                throw;
            }
        }

        /// <inheritdoc/>
        public override async Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(entity.Path))
                throw new ArgumentException("Path cannot be empty", nameof(entity));

            LogOperation("saving", entity.Id);

            try
            {
                // Build the full file path
                var relativePath = entity.Path;
                var fullPath = Path.Combine(_options.BasePath, relativePath);

                // Ensure directory exists
                var directoryPath = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrWhiteSpace(directoryPath) && !_fileSystem.Directory.Exists(directoryPath))
                {
                    if (_options.CreateDirectoriesIfNotExist)
                    {
                        _fileSystem.Directory.CreateDirectory(directoryPath);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory does not exist: {directoryPath}");
                    }
                }

                // Generate content with front matter
                var content = GenerateMarkdownWithFrontMatter(entity);

                // Write content to file
                await _fileSystem.File.WriteAllTextAsync(fullPath, content, cancellationToken);

                // Update provider-specific ID (file path in this case)
                entity.SetProviderSpecificId(fullPath);

                // Refresh cache
                await RefreshCacheAsync(cancellationToken);

                return entity;
            }
            catch (Exception ex)
            {
                LogError(ex, "saving", entity.Id);
                throw new ContentProviderException($"Failed to save content item: {ex.Message}", ex, ProviderId);
            }
        }

        /// <inheritdoc/>
        public override async Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID cannot be empty", nameof(id));

            LogOperation("deleting", id);

            try
            {
                // Get item to get path
                var item = await GetByIdAsync(id, cancellationToken);
                if (item is null)
                    throw new ContentItemNotFoundException(id, ProviderId);

                var fullPath = item.ProviderSpecificId;
                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    // If provider-specific ID is not set, try using path
                    fullPath = Path.Combine(_options.BasePath, item.Path);
                }

                if (!_fileSystem.File.Exists(fullPath))
                    throw new ContentProviderException($"Content file not found: {fullPath}", ProviderId);

                // Delete file
                _fileSystem.File.Delete(fullPath);

                // Refresh cache
                await RefreshCacheAsync(cancellationToken);
            }
            catch (ContentItemNotFoundException)
            {
                // Re-throw not found exception
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, "deleting", id);
                throw new ContentProviderException($"Failed to delete content item: {ex.Message}", ex, ProviderId);
            }
        }

        private bool IsExcluded(string filePath)
        {
            // Skip _index.md files - they're for directory metadata
            if (Path.GetFileName(filePath).Equals("_index.md", StringComparison.OrdinalIgnoreCase))
                return true;

            // Check exclude patterns
            //foreach (var pattern in _options.ExcludePatterns)
            //{
            //    if (MatchesGlobPattern(filePath, pattern))
            //        return true;
            //}

            return false;
        }

        private async Task<ContentItem?> ProcessMarkdownFileAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                // Read file content
                var content = await _fileSystem.File.ReadAllTextAsync(filePath, cancellationToken);
                if (string.IsNullOrWhiteSpace(content))
                    return null;

                // Get ID from relative path
                var relativePath = GetRelativePath(filePath);
                var id = relativePath.GetHashCode().ToString("x");

                // Create the content item
                var contentItem = ContentItem.Create(
                    id: id,
                    title: Path.GetFileNameWithoutExtension(filePath),
                    content: string.Empty, // Will be set in ProcessMarkdownAsync
                    path: relativePath,
                    providerId: ProviderId);

                // Store the provider-specific ID (file path)
                contentItem.SetProviderSpecificId(filePath);

                var fileInfo = _fileSystem.FileInfo.New(filePath);
                contentItem.SetCreatedDate(fileInfo.CreationTimeUtc);
                contentItem.SetLastModifiedDate(fileInfo.LastWriteTimeUtc);

                // Extract locale from path if enabled
                if (_options.EnableLocalization)
                {
                    var locale = UrlGenerator.ExtractLocaleFromPath(relativePath, ContentPath, EnableLocalization, SupportedLocales, DefaultLocale);
                    contentItem.SetLocale(locale);
                }
                else
                {
                    contentItem.SetLocale(_options.DefaultLocale);
                }

                // Process the markdown content
                await ProcessMarkdownAsync(content, contentItem, cancellationToken);

                // Set URL
                var url = UrlGenerator.GenerateUrl(contentItem.Path, contentItem.Slug, EnableLocalization, SupportedLocales, ContentPath);
                contentItem.SetUrl(url);

                // Set content ID if localization is enabled
                if (_options.EnableLocalization && string.IsNullOrWhiteSpace(contentItem.ContentId))
                {
                    var pathWithoutLocale = UrlGenerator.RemoveLocaleFromPath(contentItem.Path, ContentPath, EnableLocalization, SupportedLocales);
                    var pathWithoutExtension = Path.ChangeExtension(pathWithoutLocale, null);
                    contentItem.SetContentId(pathWithoutExtension.GetHashCode().ToString("x"));
                }

                return contentItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing markdown file: {FilePath}", filePath);
                return null;
            }
        }

        private string GetRelativePath(string fullPath)
        {
            return Path.GetRelativePath(_options.BasePath, fullPath).Replace('\\', '/');
        }

        private void SetupFileWatcher()
        {
            try
            {
                if (!_fileSystem.Directory.Exists(_options.BasePath))
                {
                    if (_options.CreateDirectoriesIfNotExist)
                    {
                        _fileSystem.Directory.CreateDirectory(_options.BasePath);
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
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
                    EnableRaisingEvents = true
                };

                // Add handler for all change events
                _fileWatcher.Changed += OnFileChanged;
                _fileWatcher.Created += OnFileChanged;
                _fileWatcher.Deleted += OnFileChanged;
                _fileWatcher.Renamed += OnFileChanged;

                Logger.LogInformation("File watcher set up for path: {BasePath}", _options.BasePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to set up file watcher for {BasePath}", _options.BasePath);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            var extension = Path.GetExtension(e.FullPath);
            if (_options.SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                Logger.LogInformation("File change detected: {ChangeType} - {Path}", e.ChangeType, e.FullPath);

                // Invalidate cache
                _ = RefreshCacheAsync();
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
}