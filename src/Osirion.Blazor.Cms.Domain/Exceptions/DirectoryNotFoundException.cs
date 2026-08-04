namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when a directory is not found
/// </summary>
public class DirectoryNotFoundException : DomainException
{
    /// <summary>Gets or sets the DirectoryId value.</summary>
    public string DirectoryId { get; }

    /// <summary>Gets or sets the DirectoryNotFoundException value.</summary>
    public DirectoryNotFoundException(string directoryId)
        : base($"Directory with ID '{directoryId}' was not found.")
    {
        DirectoryId = directoryId;
    }
}
