using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Core.Services;

/// <summary>
/// Advanced Markdown editing service with comprehensive features
/// </summary>
public interface IMarkdownEditorService
{
    /// <summary>
    /// Validates markdown content
    /// </summary>
    MarkdownValidationResult ValidateMarkdown(string markdown);

    /// <summary>
    /// Generates a table of contents from markdown
    /// </summary>
    string GenerateTableOfContents(string markdown);

    /// <summary>
    /// Sanitizes markdown content
    /// </summary>
    string SanitizeMarkdown(string markdown);

    /// <summary>
    /// Provides markdown formatting suggestions
    /// </summary>
    IEnumerable<MarkdownFormattingSuggestion> GetFormattingSuggestions(string markdown);
}

/// <summary>
/// Markdown validation result
/// </summary>
public class MarkdownValidationResult
{
    public bool IsValid { get; set; }
    public List<MarkdownValidationError> Errors { get; set; } = new();
}

/// <summary>
/// Markdown validation error details
/// </summary>
public class MarkdownValidationError
{
    public int LineNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public MarkdownValidationErrorSeverity Severity { get; set; }
}

/// <summary>
/// Severity of markdown validation errors
/// </summary>
public enum MarkdownValidationErrorSeverity
{
    Warning,
    Error,
    Critical
}

/// <summary>
/// Markdown formatting suggestion
/// </summary>
public class MarkdownFormattingSuggestion
{
    public string Description { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
}

/// <summary>
/// Implementation of advanced markdown editor service
/// </summary>
public class MarkdownEditorService : IMarkdownEditorService
{
    private readonly ILogger<MarkdownEditorService> _logger;

    public MarkdownEditorService(ILogger<MarkdownEditorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Validates markdown content with comprehensive checks
    /// </summary>
    public MarkdownValidationResult ValidateMarkdown(string markdown)
    {
        var result = new MarkdownValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(markdown))
        {
            result.IsValid = false;
            result.Errors.Add(new MarkdownValidationError
            {
                ErrorMessage = "Markdown content cannot be empty",
                Severity = MarkdownValidationErrorSeverity.Critical
            });
            return result;
        }

        // Check for balanced brackets
        result.Errors.AddRange(ValidateBrackets(markdown));

        // Check for broken links
        result.Errors.AddRange(ValidateLinks(markdown));

        // Check for heading hierarchy
        result.Errors.AddRange(ValidateHeadingHierarchy(markdown));

        // Update overall validation status
        result.IsValid = result.Errors.All(e => e.Severity != MarkdownValidationErrorSeverity.Critical);

        return result;
    }

    /// <summary>
    /// Generates a table of contents from markdown headings
    /// </summary>
    public string GenerateTableOfContents(string markdown)
    {
        var headingRegex = new Regex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline);
        var matches = headingRegex.Matches(markdown);

        var tocBuilder = new System.Text.StringBuilder();
        tocBuilder.AppendLine("## Table of Contents");

        foreach (Match match in matches)
        {
            var level = match.Groups[1].Value.Length;
            var title = match.Groups[2].Value.Trim();
            var slug = GenerateSlug(title);

            // Indent based on heading level
            tocBuilder.AppendLine($"{new string(' ', (level - 1) * 2)}- [{title}](#{slug})");
        }

        return tocBuilder.ToString();
    }

    /// <summary>
    /// Sanitizes markdown content to prevent potential security issues
    /// </summary>
    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove potentially dangerous HTML
        var sanitizedMarkdown = Regex.Replace(
            markdown,
            @"<\s*script.*?>.*?<\s*/\s*script\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove inline event handlers
        sanitizedMarkdown = Regex.Replace(
            sanitizedMarkdown,
            @"on\w+\s*=\s*""[^""]*""",
            "",
            RegexOptions.IgnoreCase
        );

        return sanitizedMarkdown;
    }

    /// <summary>
    /// Provides formatting suggestions for markdown
    /// </summary>
    public IEnumerable<MarkdownFormattingSuggestion> GetFormattingSuggestions(string markdown)
    {
        var suggestions = new List<MarkdownFormattingSuggestion>();

        // Check for long paragraphs
        var longParagraphRegex = new Regex(@"^([^\n]+\n){4,}", RegexOptions.Multiline);
        if (longParagraphRegex.IsMatch(markdown))
        {
            suggestions.Add(new MarkdownFormattingSuggestion
            {
                Description = "Long paragraphs detected. Consider breaking them up.",
                SuggestedFix = "Split long paragraphs into smaller, more readable chunks."
            });
        }

        // Check for repeated words
        var repeatedWordRegex = new Regex(@"\b(\w+)\s+\1\b", RegexOptions.IgnoreCase);
        if (repeatedWordRegex.IsMatch(markdown))
        {
            suggestions.Add(new MarkdownFormattingSuggestion
            {
                Description = "Repeated words found. Review for clarity.",
                SuggestedFix = "Remove or rephrase repeated words to improve readability."
            });
        }

        return suggestions;
    }

    /// <summary>
    /// Validates balanced brackets in markdown
    /// </summary>
    private IEnumerable<MarkdownValidationError> ValidateBrackets(string markdown)
    {
        var errors = new List<MarkdownValidationError>();
        var bracketsStack = new Stack<char>();

        for (int i = 0; i < markdown.Length; i++)
        {
            switch (markdown[i])
            {
                case '[':
                case '(':
                    bracketsStack.Push(markdown[i]);
                    break;
                case ']':
                    if (bracketsStack.Count == 0 || bracketsStack.Pop() != '[')
                    {
                        errors.Add(new MarkdownValidationError
                        {
                            LineNumber = GetLineNumber(markdown, i),
                            ErrorMessage = "Unbalanced square brackets",
                            Severity = MarkdownValidationErrorSeverity.Error
                        });
                    }
                    break;
                case ')':
                    if (bracketsStack.Count == 0 || bracketsStack.Pop() != '(')
                    {
                        errors.Add(new MarkdownValidationError
                        {
                            LineNumber = GetLineNumber(markdown, i),
                            ErrorMessage = "Unbalanced parentheses",
                            Severity = MarkdownValidationErrorSeverity.Error
                        });
                    }
                    break;
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates links in markdown
    /// </summary>
    private IEnumerable<MarkdownValidationError> ValidateLinks(string markdown)
    {
        var errors = new List<MarkdownValidationError>();
        var linkRegex = new Regex(@"\[([^\]]+)\]\(([^\)]+)\)", RegexOptions.Multiline);

        foreach (Match match in linkRegex.Matches(markdown))
        {
            var linkText = match.Groups[1].Value;
            var url = match.Groups[2].Value;

            // Check for empty link text
            if (string.IsNullOrWhiteSpace(linkText))
            {
                errors.Add(new MarkdownValidationError
                {
                    LineNumber = GetLineNumber(markdown, match.Index),
                    ErrorMessage = "Empty link text",
                    Severity = MarkdownValidationErrorSeverity.Warning
                });
            }

            // Check for empty or invalid URL
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                errors.Add(new MarkdownValidationError
                {
                    LineNumber = GetLineNumber(markdown, match.Index),
                    ErrorMessage = "Invalid or empty link URL",
                    Severity = MarkdownValidationErrorSeverity.Error
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates heading hierarchy
    /// </summary>
    private IEnumerable<MarkdownValidationError> ValidateHeadingHierarchy(string markdown)
    {
        var errors = new List<MarkdownValidationError>();
        var headingRegex = new Regex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline);
        var matches = headingRegex.Matches(markdown);

        int? previousLevel = null;

        foreach (Match match in matches)
        {
            var currentLevel = match.Groups[1].Value.Length;

            if (previousLevel.HasValue && currentLevel > previousLevel.Value + 1)
            {
                errors.Add(new MarkdownValidationError
                {
                    LineNumber = GetLineNumber(markdown, match.Index),
                    ErrorMessage = "Heading hierarchy is not sequential",
                    Severity = MarkdownValidationErrorSeverity.Warning
                });
            }

            previousLevel = currentLevel;
        }

        return errors;
    }

    /// <summary>
    /// Generates a URL-friendly slug from text
    /// </summary>
    private string GenerateSlug(string text)
    {
        // Remove special characters and convert to lowercase
        var slug = Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = Regex.Replace(slug, @"-{2,}", "-");

        return slug.Trim('-');
    }

    /// <summary>
    /// Gets the line number for a specific index in markdown
    /// </summary>
    private int GetLineNumber(string markdown, int index)
    {
        return markdown.Substring(0, index).Count(c => c == '\n') + 1;
    }
}