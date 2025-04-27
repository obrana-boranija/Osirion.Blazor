namespace Osirion.Blazor.Cms.Application.Validation;

/// <summary>
/// Interface for validators
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates an object
    /// </summary>
    /// <param name="instance">The object to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}