using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Directory;

/// <summary>
/// Query to get a directory by ID
/// </summary>
public class GetDirectoryByIdQuery : IQuery<DirectoryItem?>
{
    /// <summary>
    /// Gets or sets the ID of the directory to retrieve
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale filter (optional)
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}