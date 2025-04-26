namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Base exception for content providers
/// </summary>
public class ContentProviderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    public ContentProviderException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    public ContentProviderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}