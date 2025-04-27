using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get all content tags
/// </summary>
public class GetContentTagsQuery : IQuery<IReadOnlyList<ContentTag>>
{
    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}