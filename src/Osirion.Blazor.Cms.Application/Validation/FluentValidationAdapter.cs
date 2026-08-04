namespace Osirion.Blazor.Cms.Application.Validation;

/// <summary>
/// Adapter for FluentValidation validators to IValidator
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public class FluentValidationAdapter<T> : IValidator<T>
{
    private readonly FluentValidation.IValidator<T> _fluentValidator;

    /// <summary>Performs the FluentValidationAdapter operation.</summary>
    public FluentValidationAdapter(FluentValidation.IValidator<T> fluentValidator)
    {
        _fluentValidator = fluentValidator;
    }

    /// <summary>Performs the Validate operation asynchronously.</summary>
    public async Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var fluentResult = await _fluentValidator.ValidateAsync(instance, cancellationToken);

        if (fluentResult.IsValid)
        {
            return ValidationResult.Success();
        }

        var errors = fluentResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return ValidationResult.Failure(errors);
    }
}
