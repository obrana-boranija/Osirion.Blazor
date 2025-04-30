using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Content;

/// <summary>
/// Implementation of IContentQueryFilter for filtering content queries
/// </summary>
public class ContentQueryFilter : IContentQueryFilter
{
    private readonly IPathUtilities _pathUtils;

    public ContentQueryFilter(IPathUtilities pathUtils)
    {
        _pathUtils = pathUtils ?? throw new ArgumentNullException(nameof(pathUtils));
    }

    /// <inheritdoc/>
    public IQueryable<ContentItem> ApplyFilters(IQueryable<ContentItem> items, ContentQuery query)
    {
        var filteredItems = items;

        if (!string.IsNullOrEmpty(query.Directory))
        {
            var normalizedDirectory = _pathUtils.NormalizePath(query.Directory);
            filteredItems = filteredItems.Where(item =>
                _pathUtils.NormalizePath(GetDirectoryPath(item.Path)).StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.DirectoryId))
        {
            filteredItems = filteredItems.Where(item =>
                item.Directory != null && item.Directory.Id == query.DirectoryId);
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            filteredItems = filteredItems.Where(item =>
                item.Categories.Any(c => c.Equals(query.Category, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Categories != null && query.Categories.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.Categories.All(c =>
                    item.Categories.Any(itemCat =>
                        itemCat.Equals(c, StringComparison.OrdinalIgnoreCase))));
        }

        if (!string.IsNullOrEmpty(query.Tag))
        {
            filteredItems = filteredItems.Where(item =>
                item.Tags.Any(t => t.Equals(query.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Tags != null && query.Tags.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.Tags.All(t =>
                    item.Tags.Any(itemTag =>
                        itemTag.Equals(t, StringComparison.OrdinalIgnoreCase))));
        }

        if (query.IsFeatured.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
        }

        if (!string.IsNullOrEmpty(query.Author))
        {
            filteredItems = filteredItems.Where(item =>
                item.Author.Equals(query.Author, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Status.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.Status == query.Status.Value);
        }

        if (query.DateFrom.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.DateCreated >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.DateCreated <= query.DateTo.Value);
        }

        if (!string.IsNullOrEmpty(query.SearchQuery))
        {
            var searchTerms = query.SearchQuery.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            filteredItems = filteredItems.Where(item =>
                searchTerms.Any(term =>
                    (item.Title != null && item.Title.ToLower().Contains(term)) ||
                    (item.Description != null && item.Description.ToLower().Contains(term)) ||
                    (item.Content != null && item.Content.ToLower().Contains(term)) ||
                    item.Categories.Any(c => c.ToLower().Contains(term)) ||
                    item.Tags.Any(t => t.ToLower().Contains(term))
                )
            );
        }

        if (!string.IsNullOrEmpty(query.Locale))
        {
            filteredItems = filteredItems.Where(item =>
                item.Locale.Equals(query.Locale, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.LocalizationId))
        {
            filteredItems = filteredItems.Where(item =>
                item.ContentId.Equals(query.LocalizationId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.ProviderId) && query.ProviderId != items.FirstOrDefault()?.ProviderId)
        {
            // No items match this provider - return empty query
            filteredItems = filteredItems.Where(i => false);
        }

        if (query.IncludeIds != null && query.IncludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.IncludeIds.Contains(item.Id));
        }

        if (query.ExcludeIds != null && query.ExcludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                !query.ExcludeIds.Contains(item.Id));
        }

        return filteredItems;
    }

    /// <summary>
    /// Gets the directory path from a file path
    /// </summary>
    private string GetDirectoryPath(string filePath)
    {
        var lastSlashIndex = filePath.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            return filePath.Substring(0, lastSlashIndex);
        }
        return string.Empty;
    }
}