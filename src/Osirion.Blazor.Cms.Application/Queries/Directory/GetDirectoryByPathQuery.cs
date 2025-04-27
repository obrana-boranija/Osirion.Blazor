using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Application.Queries.Directory;

/// <summary>
/// Query to get a directory by path
/// </summary>
public class GetDirectoryByPathQuery : IQuery<DirectoryItem?>
{
    /// <summary>
    /// Gets or sets the path of the directory to retrieve
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }
}