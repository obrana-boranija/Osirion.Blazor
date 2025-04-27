using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get all content categories
/// </summary>
public class GetContentCategoriesQuery : IQuery<IReadOnlyList<ContentCategory>>
{
    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}