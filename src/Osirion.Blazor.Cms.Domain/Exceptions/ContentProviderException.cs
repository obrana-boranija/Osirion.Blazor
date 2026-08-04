namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when there's an issue with the content provider
/// </summary>
public class ContentProviderException : DomainException
{
    /// <summary>Gets or sets the ProviderId value.</summary>
    public string? ProviderId { get; }

    /// <summary>Gets or sets the ContentProviderException value.</summary>
    public ContentProviderException(string message, string? providerId = null)
        : base(message)
    {
        ProviderId = providerId;
    }

    /// <summary>Gets or sets the ContentProviderException value.</summary>
    public ContentProviderException(string message, Exception innerException, string? providerId = null)
        : base(message, innerException)
    {
        ProviderId = providerId;
    }
}
