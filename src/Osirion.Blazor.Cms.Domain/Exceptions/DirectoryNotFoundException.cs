namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when a directory is not found
/// </summary>
public class DirectoryNotFoundException : DomainException
{
    public string DirectoryId { get; }

    public DirectoryNotFoundException(string directoryId)
        : base($"Directory with ID '{directoryId}' was not found.")
    {
        DirectoryId = directoryId;
    }
}