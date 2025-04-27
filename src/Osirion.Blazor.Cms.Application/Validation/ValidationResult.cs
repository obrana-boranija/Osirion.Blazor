namespace Osirion.Blazor.Cms.Application.Validation;

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets whether the validation was successful
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the validation errors, grouped by property name
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success() => new(true);

    /// <summary>
    /// Creates a failed validation result with errors
    /// </summary>
    public static ValidationResult Failure(IReadOnlyDictionary<string, string[]> errors)
        => new(false, errors);

    /// <summary>
    /// Creates a failed validation result with a single error
    /// </summary>
    public static ValidationResult Failure(string propertyName, string errorMessage)
        => new(false, new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        });
}