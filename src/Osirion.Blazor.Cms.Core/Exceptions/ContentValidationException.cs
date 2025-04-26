namespace Osirion.Blazor.Cms.Exceptions;

/// <summary>
/// Exception thrown when content validation fails
/// </summary>
public class ContentValidationException : ContentProviderException
{
    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    public ContentValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    public ContentValidationException(string message, string propertyName, string error)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { error } }
        };
    }
}