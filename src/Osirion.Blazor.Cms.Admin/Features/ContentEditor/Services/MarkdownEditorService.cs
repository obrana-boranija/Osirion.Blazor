using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

/// <summary>
/// Service for rendering markdown content
/// </summary>
public class MarkdownEditorService : IMarkdownEditorService
{
    private readonly ILogger<MarkdownEditorService> _logger;
    private readonly MarkdownPipeline _pipeline;

    public MarkdownEditorService(
        IOptions<CmsAdminOptions> options,
        ILogger<MarkdownEditorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure Markdig pipeline based on options
        var builder = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAutoIdentifiers()
            .UseEmojiAndSmiley()
            .UseGridTables()
            .UseTaskLists()
            .UsePipeTables()
            .UseCitations()
            .UseFootnotes()
            .UseDiagrams();

        _pipeline = builder.Build();
    }

    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    public Task<string> RenderToHtmlAsync(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return Task.FromResult(string.Empty);
        }

        try
        {
            var html = Markdown.ToHtml(markdown, _pipeline);
            return Task.FromResult(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering markdown to HTML");

            // In case of error, return a sanitized version of the exception message
            // wrapped in a div with error styling
            return Task.FromResult(
                $"<div class=\"markdown-rendering-error\">Error rendering markdown: {System.Net.WebUtility.HtmlEncode(ex.Message)}</div>");
        }
    }

    /// <summary>
    /// Parses front matter from markdown
    /// </summary>
    public Task<Dictionary<string, object>> ParseFrontMatterAsync(string markdown)
    {
        var frontMatter = new Dictionary<string, object>();

        if (string.IsNullOrEmpty(markdown))
        {
            return Task.FromResult(frontMatter);
        }

        try
        {
            // Simple front matter parsing - assumes YAML front matter between --- delimiters
            if (markdown.StartsWith("---"))
            {
                var endIndex = markdown.IndexOf("---", 3);

                if (endIndex > 3)
                {
                    var frontMatterYaml = markdown.Substring(3, endIndex - 3).Trim();
                    var lines = frontMatterYaml.Split('\n');

                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();

                        if (string.IsNullOrEmpty(trimmedLine) || !trimmedLine.Contains(':'))
                        {
                            continue;
                        }

                        var parts = trimmedLine.Split(new[] { ':' }, 2);

                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();

                            // Strip quotes if present
                            if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
                            {
                                value = value.Substring(1, value.Length - 2);
                            }

                            frontMatter[key] = value;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing front matter from markdown");
        }

        return Task.FromResult(frontMatter);
    }

    /// <summary>
    /// Extracts the content without front matter
    /// </summary>
    public Task<string> ExtractContentAsync(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return Task.FromResult(string.Empty);
        }

        try
        {
            // If markdown starts with front matter, remove it
            if (markdown.StartsWith("---"))
            {
                var endIndex = markdown.IndexOf("---", 3);

                if (endIndex > 3)
                {
                    // Return everything after the front matter
                    return Task.FromResult(markdown.Substring(endIndex + 3).Trim());
                }
            }

            // If no front matter, return the original markdown
            return Task.FromResult(markdown);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting content from markdown");
            return Task.FromResult(markdown);
        }
    }
}