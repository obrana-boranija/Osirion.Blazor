using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get a content item by ID
/// </summary>
public class GetContentByIdQuery : IQuery<ContentItem?>
{
    /// <summary>
    /// Gets or sets the ID of the content to retrieve
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}