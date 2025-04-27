using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get a content item by path
/// </summary>
public class GetContentByPathQuery : IQuery<ContentItem?>
{
    /// <summary>
    /// Gets or sets the path of the content to retrieve
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}