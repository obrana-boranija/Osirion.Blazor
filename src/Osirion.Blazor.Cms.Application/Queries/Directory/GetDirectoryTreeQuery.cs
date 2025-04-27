using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Directory;

/// <summary>
/// Query to get the full directory tree
/// </summary>
public class GetDirectoryTreeQuery : IQuery<IReadOnlyList<DirectoryItem>>
{
    /// <summary>
    /// Gets or sets the locale filter (optional)
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}