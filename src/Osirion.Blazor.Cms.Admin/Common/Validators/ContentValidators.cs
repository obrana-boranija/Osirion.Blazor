using System.ComponentModel.DataAnnotations;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Osirion.Blazor.Cms.Admin.Common.Validators;

    /// <summary>Defines the public member type.</summary>
public static class ContentValidators
{
    /// <summary>Gets or sets the ValidateFileName value.</summary>
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

    /// <summary>Gets or sets the ValidateBranchName value.</summary>
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

    /// <summary>Performs the ValidateFrontMatter operation.</summary>
    public static ValidationResult? ValidateFrontMatter(FrontMatter frontMatter, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(frontMatter.Title))
        {
            return new ValidationResult("Title is required", new[] { nameof(frontMatter.Title) });
        }

        return ValidationResult.Success;
    }

    /// <summary>Performs the ValidateBlogPost operation.</summary>
    public static ValidationResult? ValidateBlogPost(ContentItem blogPost, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(blogPost.Content))
        {
            return new ValidationResult("Content is required", new[] { nameof(blogPost.Content) });
        }

        return blogPost.Metadata is { } frontMatter
            ? ValidateFrontMatter(frontMatter, context)
            : new ValidationResult("Front matter is required", new[] { nameof(blogPost.Metadata) });
    }
}
