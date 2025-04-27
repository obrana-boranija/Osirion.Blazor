namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Core interface for content providers, composing multiple content interfaces
/// </summary>
public interface IContentProviderInterface : IContentReader, IContentQuerying, IContentCaching
{
    /// <summary>
    /// Gets the unique identifier for the provider
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Gets the display name for the provider
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets whether the provider is read-only or supports write operations
    /// </summary>
    bool IsReadOnly { get; }
}