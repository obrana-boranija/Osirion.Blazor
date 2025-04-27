namespace Osirion.Blazor.Cms.Domain.Models;

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation was successful
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; private set; } = new();

    /// <summary>
    /// Adds an error for a specific property
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="errorMessage">Error message</param>
    public void AddError(string propertyName, string errorMessage)
    {
        IsValid = false;

        if (!Errors.TryGetValue(propertyName, out var propertyErrors))
        {
            propertyErrors = new List<string>();
            Errors[propertyName] = propertyErrors;
        }

        propertyErrors.Add(errorMessage);
    }

    /// <summary>
    /// Gets all error messages as a flattened list
    /// </summary>
    public List<string> GetAllErrors()
    {
        return Errors
            .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
            .ToList();
    }

    /// <summary>
    /// Creates a success validation result
    /// </summary>
    public static ValidationResult Success() => new ValidationResult { IsValid = true };

    /// <summary>
    /// Creates a failure validation result with a single error
    /// </summary>
    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        var result = new ValidationResult { IsValid = false };
        result.AddError(propertyName, errorMessage);
        return result;
    }

    /// <summary>
    /// Creates a failure validation result with multiple errors
    /// </summary>
    public static ValidationResult Failure(Dictionary<string, List<string>> errors)
    {
        var result = new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
        return result;
    }
}