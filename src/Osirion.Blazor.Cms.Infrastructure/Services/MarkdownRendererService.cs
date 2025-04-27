using Markdig;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class MarkdownRendererService : IMarkdownRendererService
{
    public string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        var pipelineBuilder = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions();

        configureMarkdig?.Invoke(pipelineBuilder);
        var pipeline = pipelineBuilder.Build();

        return Markdig.Markdown.ToHtml(markdown, pipeline);
    }

    public Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null)
    {
        // Async wrapper for the synchronous method
        return Task.FromResult(RenderToHtml(markdown, configureMarkdig));
    }

    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        // Basic sanitization implementation
        // In a real app, you would want to use a proper HTML sanitizer
        return markdown;
    }
}