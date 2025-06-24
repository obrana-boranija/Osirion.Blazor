namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Interface for content providers that can update their cache when changes are detected
/// </summary>
public interface IContentCacheUpdater
{
    /// <summary>
    /// Gets the provider ID
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Updates the content cache with the latest changes
    /// </summary>
    /// <param name="latestSha">The latest commit SHA (if available)</param>
    /// <param name="forceBackground">Force Background</param>
    /// <returns>Task that completes when the cache is updated</returns>
    Task UpdateCacheAsync(string? latestSha = null, bool forceBackground = false);
}