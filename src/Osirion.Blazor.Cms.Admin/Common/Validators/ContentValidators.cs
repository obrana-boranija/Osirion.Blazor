using System.ComponentModel.DataAnnotations;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Osirion.Blazor.Cms.Admin.Common.Validators;

public static class ContentValidators
{
    public static ValidationResult? ValidateFileName(string fileName, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return new ValidationResult("File name cannot be empty");
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.Any(c => invalidChars.Contains(c)))
        {
            return new ValidationResult($"File name contains invalid characters: {string.Join(", ", invalidChars)}");
        }

        // Ensure file has .md extension
        if (!fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult("File name must end with .md extension");
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateBranchName(string branchName, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(branchName))
        {
            return new ValidationResult("Branch name cannot be empty");
        }

        // Branch name rules: cannot contain spaces, ~, ^, :, \, ?, *, [, cannot start or end with /
        if (branchName.Contains(' ') ||
            branchName.Contains('~') ||
            branchName.Contains('^') ||
            branchName.Contains(':') ||
            branchName.Contains('\\') ||
            branchName.Contains('?') ||
            branchName.Contains('*') ||
            branchName.Contains('[') ||
            branchName.StartsWith('/') ||
            branchName.EndsWith('/'))
        {
            return new ValidationResult("Branch name contains invalid characters");
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateFrontMatter(FrontMatter frontMatter, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(frontMatter.Title))
        {
            return new ValidationResult("Title is required", new[] { nameof(frontMatter.Title) });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateBlogPost(ContentItem blogPost, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(blogPost.Content))
        {
            return new ValidationResult("Content is required", new[] { nameof(blogPost.Content) });
        }

        return ValidateFrontMatter(blogPost.Metadata, context);
    }
}