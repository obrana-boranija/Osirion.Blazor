using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Repository implementation for GitHub content
/// </summary>
public class GitHubContentRepository : BaseContentRepository
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly GitHubOptions _options;
    private string _lastKnownCommitSha = string.Empty;

    public GitHubContentRepository(
        IGitHubApiClient apiClient,
        IMarkdownProcessor markdownProcessor,
        IOptions<GitHubOptions> options,
        IDirectoryRepository directoryRepository,
        ILogger<GitHubContentRepository> logger)
        : base(GetProviderId(options.Value), markdownProcessor, logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Set base class properties
        CacheDurationMinutes = _options.CacheDurationMinutes;
        EnableLocalization = _options.EnableLocalization;
        DefaultLocale = _options.DefaultLocale;
        SupportedLocales = _options.SupportedLocales;
        ContentPath = _options.ContentPath;

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
    protected override async Task<Dictionary<string, ContentItem>> LoadItemsIntoCache(CancellationToken cancellationToken)
    {
        var cache = new Dictionary<string, ContentItem>();
        var contentPath = NormalizePath(_options.ContentPath);

        try
        {
            // Get repository contents
            Logger.LogInformation("Loading GitHub content from {Owner}/{Repository} path: {Path}",
                _options.Owner, _options.Repository, contentPath);

            var contents = await _apiClient.GetRepositoryContentsAsync(contentPath, cancellationToken);

            // Update last known commit SHA if available
            try
            {
                var branches = await _apiClient.GetBranchesAsync(cancellationToken);
                var branch = branches.FirstOrDefault(b => b.Name == _options.Branch);
                if (branch != null)
                {
                    _lastKnownCommitSha = branch.Commit.Sha;
                    Logger.LogInformation("Updated commit SHA to {Sha}", _lastKnownCommitSha);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to update commit SHA");
            }

            // Process contents recursively
            await ProcessContentsRecursivelyAsync(contents, cache, cancellationToken);

            Logger.LogInformation("Loaded {Count} content items from GitHub repository {Owner}/{Repository}",
                cache.Count, _options.Owner, _options.Repository);

            return cache;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading content from GitHub repository {Owner}/{Repository}",
                _options.Owner, _options.Repository);

            // Return empty cache rather than null
            return new Dictionary<string, ContentItem>();
        }
    }

    /// <inheritdoc/>
    public override async Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (string.IsNullOrEmpty(entity.Path))
            throw new ArgumentException("Path cannot be empty", nameof(entity));

        LogOperation("saving", entity.Id);

        try
        {
            // Generate content with front matter
            var content = GenerateMarkdownWithFrontMatter(entity);

            // Get SHA if updating
            string? sha = entity.ProviderSpecificId;

            // Create or update file
            var response = await _apiClient.CreateOrUpdateFileAsync(
                entity.Path,
                content,
                commitMessage,
                sha,
                cancellationToken);

            if (!response.Success)
            {
                throw new ContentProviderException($"Failed to save content: {response.ErrorMessage}", ProviderId);
            }

            // Update with provider-specific information
            entity.SetProviderSpecificId(response.Content.Sha);

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
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        LogOperation("deleting", id);

        try
        {
            // Get item to get path and SHA
            var item = await GetByIdAsync(id, cancellationToken);
            if (item == null)
                throw new ContentItemNotFoundException(id, ProviderId);

            if (string.IsNullOrEmpty(item.ProviderSpecificId))
                throw new ContentProviderException($"Content has no SHA; cannot delete", ProviderId);

            // Delete file
            var response = await _apiClient.DeleteFileAsync(
                item.Path,
                commitMessage,
                item.ProviderSpecificId,
                cancellationToken);

            if (!response.Success)
            {
                throw new ContentProviderException($"Failed to delete content: {response.ErrorMessage}", ProviderId);
            }

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

    private async Task ProcessContentsRecursivelyAsync(
        List<GitHubItem> contents,
        Dictionary<string, ContentItem> contentItems,
        CancellationToken cancellationToken = default)
    {
        foreach (var item in contents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item.IsFile && IsMarkdownFile(item.Name) && !item.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var fileContent = await _apiClient.GetFileContentAsync(item.Path, cancellationToken);
                    var contentItem = await ProcessMarkdownFileAsync(fileContent, cancellationToken);

                    if (contentItem != null)
                    {
                        contentItems[contentItem.Id] = contentItem;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing file: {FileName}", item.Name);
                }
            }
            else if (item.IsDirectory)
            {
                var subContents = await _apiClient.GetRepositoryContentsAsync(item.Path, cancellationToken);
                await ProcessContentsRecursivelyAsync(subContents, contentItems, cancellationToken);
            }
        }
    }

    private async Task<ContentItem?> ProcessMarkdownFileAsync(GitHubFileContent fileContent, CancellationToken cancellationToken = default)
    {
        if (fileContent == null || !IsMarkdownFile(fileContent.Name))
            return null;

        // Skip _index.md files - they're for directory metadata
        if (fileContent.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase))
            return null;

        // Get file content
        string markdownContent = fileContent.GetDecodedContent();
        if (string.IsNullOrWhiteSpace(markdownContent))
            return null;

        // Get unique ID from path
        var id = fileContent.Path.GetHashCode().ToString("x");

        // Create the content item
        var contentItem = ContentItem.Create(
            id: id,
            title: Path.GetFileNameWithoutExtension(fileContent.Name),
            content: string.Empty, // Will be set in ProcessMarkdownAsync
            path: fileContent.Path,
            providerId: ProviderId);

        // Store the provider-specific ID (SHA)
        contentItem.SetProviderSpecificId(fileContent.Sha);

        // Get file history for dates (if needed)
        //try
        //{
        //    var commit = await _apiClient.GetCommitForPathAsync(fileContent.Path, cancellationToken);
        //    if (commit != null)
        //    {
        //        contentItem.SetLastModifiedDate(commit.Committer.Date);
        //        contentItem.SetCreatedDate(commit.Author.Date);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Logger.LogWarning(ex, "Failed to get file history for {Path}, using current date", fileContent.Path);
        //    contentItem.SetCreatedDate(DateTime.UtcNow);
        //}

        // Extract locale from path if enabled
        if (_options.EnableLocalization)
        {
            var locale = ExtractLocaleFromPath(fileContent.Path);
            contentItem.SetLocale(locale);
        }
        else
        {
            contentItem.SetLocale(_options.DefaultLocale);
        }

        // Set directory if found
        try
        {
            var directoryPath = GetDirectoryPath(fileContent.Path);
            var directory = await _directoryRepository.GetByPathAsync(directoryPath, cancellationToken);
            if (directory != null)
            {
                contentItem.SetDirectory(directory);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to set directory for content: {Path}", fileContent.Path);
        }

        // Process the markdown content
        await ProcessMarkdownAsync(markdownContent, contentItem, cancellationToken);

        // Set URL
        var url = GenerateUrl(contentItem.Path, contentItem.Slug, _options.ContentPath);
        contentItem.SetUrl(url);

        // Set content ID if localization is enabled
        if (_options.EnableLocalization && string.IsNullOrEmpty(contentItem.ContentId))
        {
            var pathWithoutLocale = RemoveLocaleFromPath(contentItem.Path);
            var pathWithoutExtension = Path.ChangeExtension(pathWithoutLocale, null);
            contentItem.SetContentId(pathWithoutExtension.GetHashCode().ToString("x"));
        }

        return contentItem;
    }

    private bool IsMarkdownFile(string fileName)
    {
        return _options.SupportedExtensions.Any(ext =>
            fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}