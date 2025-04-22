namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown by content providers
/// </summary>
public class ContentProviderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    /// <param name="message">The error message</param>
    public ContentProviderException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderException"/> class.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ContentProviderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}