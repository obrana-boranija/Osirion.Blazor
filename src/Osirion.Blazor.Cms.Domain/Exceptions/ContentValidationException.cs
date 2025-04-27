namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ContentValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ContentValidationException(string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    public ContentValidationException(string propertyName, string errorMessage)
        : base($"Validation failed: {propertyName} - {errorMessage}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}