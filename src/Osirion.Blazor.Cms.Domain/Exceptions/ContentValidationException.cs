namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ContentValidationException : DomainException
{
    /// <summary>Gets or sets the Errors value.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>Gets or sets the ContentValidationException value.</summary>
    public ContentValidationException(string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    /// <summary>Gets or sets the ContentValidationException value.</summary>
    public ContentValidationException(string propertyName, string errorMessage)
        : base($"Validation failed: {propertyName} - {errorMessage}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}
