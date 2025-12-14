using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
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

        if (CategoryOrTagHasNoValue(query.Category, query.Tag))
        {
            if (!string.IsNullOrWhiteSpace(query.Path))
            {
                filteredItems = filteredItems.Where(item => item.Path.Contains(_pathUtils.NormalizePath(GetDirectoryPath(item.Path))));
            }

            if (!string.IsNullOrWhiteSpace(query.Directory))
            {
                var normalizedDirectory = query.Directory.ToLowerInvariant();
                if (query.IncludeSubdirectories)
                {
                    // Include all items in this directory and its subdirectories
                    filteredItems = filteredItems.Where(item =>
                        _pathUtils.NormalizePath(GetDirectoryPath(item.Path)).StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    // Only include items directly in this directory
                    filteredItems = filteredItems.Where(item =>
                        item.Directory != null && item.Directory.Name.Equals(normalizedDirectory, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (!string.IsNullOrWhiteSpace(query.DirectoryId))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Directory != null && item.Directory.Id == query.DirectoryId);
            }

            if (!string.IsNullOrWhiteSpace(query.Slug))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Slug != null && item.Slug == query.Slug);
            }

            if (!string.IsNullOrWhiteSpace(query.Url))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Url != null && item.Url.Equals(query.Url.Trim('/'), StringComparison.OrdinalIgnoreCase));
            } 
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var normalizedCategory = query.Category.Replace("-", " ").Trim();
            filteredItems = filteredItems.Where(item =>
                item.Categories.Any(c => c.Trim().Contains(normalizedCategory, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Categories is not null && query.Categories.Any())
        {
            var normalizedCategories = query.Categories.Select(c => c.Trim()).ToList();
            filteredItems = filteredItems.Where(item =>
                normalizedCategories.All(c =>
                    item.Categories.Any(itemCat =>
                        itemCat.Trim().Equals(c, StringComparison.OrdinalIgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            var normalizedTag = query.Tag.Trim();
            filteredItems = filteredItems.Where(item =>
                item.Tags.Any(t => t.Trim().Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)));
        }

        if (query.Tags is not null && query.Tags.Any())
        {
            var normalizedTags = query.Tags.Select(t => t.Trim()).ToList();
            filteredItems = filteredItems.Where(item =>
                normalizedTags.All(t =>
                    item.Tags.Any(itemTag =>
                        itemTag.Trim().Equals(t, StringComparison.OrdinalIgnoreCase))));
        }

        if (query.IsFeatured.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.IsFeatured == query.IsFeatured.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Author))
        {
            filteredItems = filteredItems.Where(item =>
                item.Author.Equals(query.Author, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Status.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.Status == query.Status.Value);
        }
        else if (!query.IncludeUnpublished)
        {
            // If not including unpublished, only show published content by default
            filteredItems = filteredItems.Where(item => item.Status == ContentStatus.Published);
        }

        if (query.DateFrom.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.DateCreated >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            filteredItems = filteredItems.Where(item => item.DateCreated <= query.DateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchQuery))
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

        if (!string.IsNullOrWhiteSpace(query.Locale))
        {
            filteredItems = filteredItems.Where(item =>
                item.Locale.Equals(query.Locale, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.LocalizationId))
        {
            filteredItems = filteredItems.Where(item =>
                item.ContentId.Equals(query.LocalizationId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.ProviderId) && query.ProviderId != items.FirstOrDefault()?.ProviderId)
        {
            // No items match this provider - return empty query
            filteredItems = filteredItems.Where(i => false);
        }

        if (query.IncludeIds is not null && query.IncludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                query.IncludeIds.Contains(item.Id));
        }

        if (query.ExcludeIds is not null && query.ExcludeIds.Any())
        {
            filteredItems = filteredItems.Where(item =>
                !query.ExcludeIds.Contains(item.Id));
        }

        // Apply sorting
        filteredItems = query.SortBy switch
        {
            SortField.Title => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.Title)
                : filteredItems.OrderByDescending(item => item.Title),
            
            SortField.Author => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.Author)
                : filteredItems.OrderByDescending(item => item.Author),
            
            SortField.LastModified => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.LastModified ?? item.PublishDate)
                : filteredItems.OrderByDescending(item => item.LastModified ?? item.PublishDate),
            
            SortField.Created => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.DateCreated)
                : filteredItems.OrderByDescending(item => item.DateCreated),
            
            SortField.Order => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.OrderIndex)
                : filteredItems.OrderByDescending(item => item.OrderIndex),
            
            SortField.PublishDate => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.PublishDate)
                : filteredItems.OrderByDescending(item => item.PublishDate),
            
            SortField.Slug => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.Slug)
                : filteredItems.OrderByDescending(item => item.Slug),
            
            SortField.ReadTime => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.ReadTimeMinutes)
                : filteredItems.OrderByDescending(item => item.ReadTimeMinutes),
            
            SortField.Date => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.DateCreated)
                : filteredItems.OrderByDescending(item => item.DateCreated),
            
            _ => query.SortDirection == SortDirection.Ascending
                ? filteredItems.OrderBy(item => item.DateCreated)
                : filteredItems.OrderByDescending(item => item.DateCreated)
        };

        // Apply pagination
        if (query.Skip.HasValue && query.Skip.Value > 0)
        {
            filteredItems = filteredItems.Skip(query.Skip.Value);
        }

        if (query.Take.HasValue && query.Take.Value > 0)
        {
            filteredItems = filteredItems.Take(query.Take.Value);
        }

        return filteredItems;
    }

    /// <summary>
    /// Gets the directory path from a file path
    /// </summary>
    private string GetDirectoryPath(string? filePath)
    {
        if(string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        var lastSlashIndex = filePath.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            return filePath.Substring(0, lastSlashIndex);
        }

        return string.Empty;
    }

    private bool CategoryOrTagHasNoValue(string? category, string? tag)
    {
        return string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(tag);
    }
}