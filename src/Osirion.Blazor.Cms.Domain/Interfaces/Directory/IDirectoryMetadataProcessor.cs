using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Directory;

/// <summary>
/// Processes directory metadata
/// </summary>
public interface IDirectoryMetadataProcessor
{
    /// <summary>
    /// Extracts and applies metadata to a directory
    /// </summary>
    /// <param name="directory">The directory to update</param>
    /// <param name="metadataContent">Metadata content (e.g., from _index.md)</param>
    /// <returns>Updated directory</returns>
    DirectoryItem ProcessMetadata(DirectoryItem directory, string metadataContent);

    /// <summary>
    /// Generates metadata content from a directory
    /// </summary>
    /// <param name="directory">The directory with metadata</param>
    /// <returns>Formatted metadata content</returns>
    string GenerateMetadataContent(DirectoryItem directory);
}