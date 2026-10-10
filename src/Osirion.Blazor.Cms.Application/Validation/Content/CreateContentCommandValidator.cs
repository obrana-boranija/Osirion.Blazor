using FluentValidation;
using Osirion.Blazor.Cms.Application.Commands.Content;

namespace Osirion.Blazor.Cms.Application.Validation.Content;

/// <summary>
/// Validator for CreateContentCommand
/// </summary>
public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
{
    /// <summary>Performs the CreateContentCommandValidator operation.</summary>
    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Path is required")
            .Must(ContentPathRules.IsValidRelativePath).WithMessage(ContentPathRules.InvalidPathMessage);

        RuleFor(x => x.Slug)
            .Must(BeValidSlug).When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");
    }

    private bool BeValidSlug(string? slug)
    {
        return !string.IsNullOrWhiteSpace(slug) &&
               System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }
}
