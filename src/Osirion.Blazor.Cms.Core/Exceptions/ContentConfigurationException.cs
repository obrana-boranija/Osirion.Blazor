namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when there is a problem with the configuration
/// </summary>
public class ContentConfigurationException : ContentProviderException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentConfigurationException"/> class.
    /// </summary>
    public ContentConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentConfigurationException"/> class.
    /// </summary>
    public ContentConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}