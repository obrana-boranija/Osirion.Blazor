using Markdig;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Core.Services;
/// <summary>
/// Markdown renderer service for Blazor applications
/// </summary>
/// <remarks>
/// This service provides methods to render markdown content to HTML with enhanced security and sanitization.
/// It uses the Markdig library for markdown processing and includes methods for both synchronous and asynchronous rendering.
/// </remarks>
public interface IMarkdownRendererService
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    /// <param name="markdown">Markdown content to render</param>
    /// <param name="configureMarkdig">Optional action to configure the Markdig pipeline</param>
    /// <returns>Rendered HTML</returns>
    string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null);

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    /// <param name="markdown">Markdown content to render</param>
    /// <param name="configureMarkdig">Optional action to configure the Markdig pipeline</param>
    /// <returns>Rendered HTML</returns>
    Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null);

    /// <summary>
    /// Sanitizes markdown content to remove potentially harmful content
    /// </summary>
    /// <param name="markdown">Markdown content to sanitize</param>
    /// <returns>Sanitized markdown</returns>
    string SanitizeMarkdown(string markdown);
}

/// <summary>
/// Implementation of IMarkdownRendererService
/// </summary>
public class MarkdownRendererService : IMarkdownRendererService
{
    private readonly ILogger<MarkdownRendererService> _logger;
    private readonly MarkdownPipeline _defaultPipeline;

    /// <summary>
    /// Initializes a new instance of the MarkdownRendererService
    /// </summary>
    public MarkdownRendererService(ILogger<MarkdownRendererService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create secure default pipeline
        _defaultPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .DisableHtml() // Prevent raw HTML injection
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .UseEmojiAndSmiley()
            .Build();
    }

    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    public string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        try
        {
            // Sanitize the markdown to remove potentially harmful content
            markdown = SanitizeMarkdown(markdown);

            // Use custom pipeline if provided, otherwise use default
            if (configureMarkdig != null)
            {
                var pipelineBuilder = new MarkdownPipelineBuilder();
                configureMarkdig(pipelineBuilder);

                // Always disable HTML for security
                pipelineBuilder.DisableHtml();

                var pipeline = pipelineBuilder.Build();
                return Markdig.Markdown.ToHtml(markdown, pipeline);
            }

            // Use default pipeline
            return Markdig.Markdown.ToHtml(markdown, _defaultPipeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering markdown: {Message}", ex.Message);

            // Return a safe error message
            return FormatErrorMessage(ex.Message);
        }
    }

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    public async Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        // Use Task.Run to offload CPU-intensive rendering to a background thread
        return await Task.Run(() => RenderToHtml(markdown, configureMarkdig));
    }

    /// <summary>
    /// Sanitizes markdown to remove potentially harmful content
    /// </summary>
    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove potentially dangerous scripts and HTML
        markdown = Regex.Replace(
            markdown,
            @"<\s*script.*?>.*?<\s*/\s*script\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove inline event handlers
        markdown = Regex.Replace(
            markdown,
            @"on\w+\s*=\s*""[^""]*""",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove dangerous iframe tags
        markdown = Regex.Replace(
            markdown,
            @"<\s*iframe.*?>.*?<\s*/\s*iframe\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove object and embed tags
        markdown = Regex.Replace(
            markdown,
            @"<\s*(object|embed).*?>.*?<\s*/\s*(object|embed)\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Sanitize links - remove javascript: protocol
        markdown = Regex.Replace(
            markdown,
            @"\[([^\]]*)\]\(javascript:[^\)]*\)",
            "[$1](#)",
            RegexOptions.IgnoreCase
        );

        return markdown;
    }

    /// <summary>
    /// Formats an error message to be displayed safely in HTML
    /// </summary>
    private string FormatErrorMessage(string message)
    {
        // Sanitize the error message to prevent XSS
        var sanitizedMessage = System.Web.HttpUtility.HtmlEncode(message);

        return $@"
                <div class=""markdown-error"">
                    <p>There was an error rendering this markdown content.</p>
                    <pre>{sanitizedMessage}</pre>
                </div>";
    }
}