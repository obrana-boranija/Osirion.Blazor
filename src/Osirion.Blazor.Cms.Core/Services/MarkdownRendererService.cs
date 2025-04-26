using Markdig;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Services;

/// <summary>
/// Implementation of IMarkdownRendererService
/// </summary>
public class MarkdownRendererService : IMarkdownRendererService
{
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<MarkdownRendererService> _logger;

    /// <summary>
    /// Initializes a new instance of the MarkdownRendererService
    /// </summary>
    public MarkdownRendererService(
        IMarkdownProcessor markdownProcessor,
        ILogger<MarkdownRendererService> logger)
    {
        _markdownProcessor = markdownProcessor ?? throw new ArgumentNullException(nameof(markdownProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    public string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        return _markdownProcessor.RenderToHtml(markdown);
    }

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    public async Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        return await _markdownProcessor.RenderToHtmlAsync(markdown);
    }

    /// <summary>
    /// Sanitizes markdown to remove potentially harmful content
    /// </summary>
    public string SanitizeMarkdown(string markdown)
    {
        return _markdownProcessor.SanitizeMarkdown(markdown);
    }
}