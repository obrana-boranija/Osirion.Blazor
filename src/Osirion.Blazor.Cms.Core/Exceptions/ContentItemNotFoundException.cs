namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when a content item is not found
/// </summary>
public class ContentItemNotFoundException : ContentProviderException
{
    /// <summary>
    /// Gets the item ID that was not found
    /// </summary>
    public string ItemId { get; }

    /// <summary>
    /// Gets the provider ID where the item was not found
    /// </summary>
    public string? ProviderId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentItemNotFoundException"/> class.
    /// </summary>
    public ContentItemNotFoundException(string itemId, string? providerId = null)
        : base($"Content item with ID '{itemId}' not found{(providerId != null ? $" in provider '{providerId}'" : "")}.")
    {
        ItemId = itemId;
        ProviderId = providerId;
    }
}