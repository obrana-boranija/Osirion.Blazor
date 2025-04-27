using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to search for content items
/// </summary>
public class SearchContentQuery : IQuery<IReadOnlyList<ContentItem>>
{
    /// <summary>
    /// Gets or sets the content query object with search criteria
    /// </summary>
    public ContentQuery Query { get; set; } = new();

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}