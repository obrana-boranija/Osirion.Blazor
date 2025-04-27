using FluentValidation;
using Osirion.Blazor.Cms.Application.Commands.Content;

namespace Osirion.Blazor.Cms.Application.Validation.Content;

/// <summary>
/// Validator for CreateContentCommand
/// </summary>
public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
{
    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Path is required")
            .Must(BeValidPath).WithMessage("Path contains invalid characters");

        RuleFor(x => x.Slug)
            .Must(BeValidSlug).When(x => !string.IsNullOrEmpty(x.Slug))
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");
    }

    private bool BeValidPath(string path)
    {
        return !string.IsNullOrEmpty(path) && !path.Contains("..") &&
               !path.Contains("\\") && !path.Contains("?") &&
               !path.Contains("*") && !path.Contains(":") &&
               !path.Contains("<") && !path.Contains(">");
    }

    private bool BeValidSlug(string slug)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }
}