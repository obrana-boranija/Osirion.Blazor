namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when a content item is not found
/// </summary>
public class ContentItemNotFoundException : DomainException
{
    /// <summary>Gets or sets the ContentId value.</summary>
    public string ContentId { get; }
    /// <summary>Gets or sets the ProviderType value.</summary>
    public string? ProviderType { get; }

    /// <summary>Gets or sets the ContentItemNotFoundException value.</summary>
    public ContentItemNotFoundException(string contentId, string? providerType = null)
        : base($"Content item with ID '{contentId}' was not found{(providerType is not null ? $" in provider '{providerType}'" : "")}.")
    {
        ContentId = contentId;
        ProviderType = providerType;
    }
}
