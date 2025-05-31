namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when a content item is not found
/// </summary>
public class ContentItemNotFoundException : DomainException
{
    public string ContentId { get; }
    public string? ProviderType { get; }

    public ContentItemNotFoundException(string contentId, string? providerType = null)
        : base($"Content item with ID '{contentId}' was not found{(providerType is not null ? $" in provider '{providerType}'" : "")}.")
    {
        ContentId = contentId;
        ProviderType = providerType;
    }
}