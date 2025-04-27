using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Content;

/// <summary>
/// Query to get a content item by URL
/// </summary>
public class GetContentByUrlQuery : IQuery<ContentItem?>
{
    /// <summary>
    /// Gets or sets the URL of the content to retrieve
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}