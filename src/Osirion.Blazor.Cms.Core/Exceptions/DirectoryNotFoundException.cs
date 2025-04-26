namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when a directory is not found
/// </summary>
public class DirectoryNotFoundException : ContentProviderException
{
    /// <summary>
    /// Gets the directory path that was not found
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryNotFoundException"/> class.
    /// </summary>
    public DirectoryNotFoundException(string directoryPath)
        : base($"Directory with path '{directoryPath}' not found.")
    {
        DirectoryPath = directoryPath;
    }
}