namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when there's an issue with the content provider
/// </summary>
public class ContentProviderException : DomainException
{
    public string? ProviderId { get; }

    public ContentProviderException(string message, string? providerId = null)
        : base(message)
    {
        ProviderId = providerId;
    }

    public ContentProviderException(string message, Exception innerException, string? providerId = null)
        : base(message, innerException)
    {
        ProviderId = providerId;
    }
}