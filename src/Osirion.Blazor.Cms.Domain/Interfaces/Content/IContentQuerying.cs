using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Interface for querying and filtering content items
/// </summary>
public interface IContentQuerying
{
    /// <summary>
    /// Gets content items based on a query
    /// </summary>
    Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content items in a directory
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags from the provider
    /// </summary>
    Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories from the provider
    /// </summary>
    Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}