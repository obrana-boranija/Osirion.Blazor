namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when there is a problem with the file system
/// </summary>
public class ContentFileSystemException : ContentProviderException
{
    /// <summary>
    /// Gets the path that caused the exception
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFileSystemException"/> class.
    /// </summary>
    public ContentFileSystemException(string message, string filePath)
        : base(message)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFileSystemException"/> class.
    /// </summary>
    public ContentFileSystemException(string message, string filePath, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }
}