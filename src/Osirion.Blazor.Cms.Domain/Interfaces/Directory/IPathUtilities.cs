namespace Osirion.Blazor.Cms.Domain.Interfaces.Directory;

/// <summary>
/// Utilities for path manipulation
/// </summary>
public interface IPathUtilities
{
    /// <summary>
    /// Normalizes a path for consistent comparison
    /// </summary>
    /// <param name="path">Path to normalize</param>
    /// <returns>Normalized path</returns>
    string NormalizePath(string path);

    /// <summary>
    /// Extracts locale from a path based on configuration
    /// </summary>
    /// <param name="path">Path to extract from</param>
    /// <returns>Extracted locale or default</returns>
    string ExtractLocaleFromPath(string path);

    /// <summary>
    /// Determines if a path matches a glob pattern
    /// </summary>
    /// <param name="path">Path to check</param>
    /// <param name="pattern">Glob pattern</param>
    /// <returns>True if pattern matches</returns>
    bool MatchesGlobPattern(string path, string pattern);

    /// <summary>
    /// Removes locale prefix from a path
    /// </summary>
    /// <param name="path">Path with possible locale prefix</param>
    /// <returns>Path without locale prefix</returns>
    string RemoveLocaleFromPath(string path);

    /// <summary>
    /// Generates a URL for a directory
    /// </summary>
    /// <param name="path">Path of the directory</param>
    /// <returns>URL friendly string</returns>
    string GenerateDirectoryUrl(string path);
}