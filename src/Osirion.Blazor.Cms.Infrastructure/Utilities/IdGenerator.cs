using System.Security.Cryptography;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Utilities;

/// <summary>
/// Provides utilities for generating stable, collision-resistant IDs
/// </summary>
public static class IdGenerator
{
    /// <summary>
    /// Generates a stable ID from a path and other parameters
    /// </summary>
    /// <param name="path">The path to use as primary identifier</param>
    /// <param name="additionalComponents">Optional additional components to make the ID more unique</param>
    /// <returns>A stable, collision-resistant ID string</returns>
    public static string GenerateStableId(string path, params string[] additionalComponents)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        // Combine path with additional components
        var builder = new StringBuilder(path);
        foreach (var component in additionalComponents)
        {
            if (!string.IsNullOrWhiteSpace(component))
            {
                builder.Append('|');
                builder.Append(component);
            }
        }

        var input = builder.ToString();

        // Use SHA256 for stable, collision-resistant hash
        using (var sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);

            // Use first 8 bytes (16 hex chars) for the ID
            return BitConverter.ToString(hash, 0, 8).Replace("-", "").ToLowerInvariant();
        }
    }

    /// <summary>
    /// Generates a stable ID specifically for directory items
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="name">Directory name</param>
    /// <param name="providerId">Provider ID</param>
    /// <returns>A stable directory ID</returns>
    public static string GenerateDirectoryId(string path, string name, string providerId)
    {
        return GenerateStableId(path, name, providerId);
    }

    /// <summary>
    /// Generates a stable ID specifically for content items
    /// </summary>
    /// <param name="path">Content path</param>
    /// <param name="title">Content title</param>
    /// <param name="providerId">Provider ID</param>
    /// <returns>A stable content ID</returns>
    public static string GenerateContentId(string path, string title, string providerId)
    {
        return GenerateStableId(path, title, providerId);
    }
}