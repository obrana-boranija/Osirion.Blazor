using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get all content items
/// </summary>
public class GetAllContentQuery : IQuery<IReadOnlyList<ContentItem>>
{
    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}