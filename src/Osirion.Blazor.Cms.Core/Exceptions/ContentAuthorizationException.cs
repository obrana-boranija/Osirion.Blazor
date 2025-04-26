namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when the user is not authorized to perform an operation
/// </summary>
public class ContentAuthorizationException : ContentProviderException
{
    /// <summary>
    /// Gets the required permission that was missing
    /// </summary>
    public string RequiredPermission { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentAuthorizationException"/> class.
    /// </summary>
    public ContentAuthorizationException(string message, string requiredPermission)
        : base(message)
    {
        RequiredPermission = requiredPermission;
    }
}