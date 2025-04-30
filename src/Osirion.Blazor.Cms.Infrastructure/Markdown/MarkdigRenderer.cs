using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Implementation of IMarkdownRenderer using Markdig library
/// </summary>
public class MarkdigRenderer : IMarkdownRenderer
{
    private readonly MarkdownPipeline _defaultPipeline;

    public MarkdigRenderer()
    {
        _defaultPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    public ObjectRendererCollection ObjectRenderers => throw new NotImplementedException();

    public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;
    public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;

    public object Render(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string RenderToHtml(string markdown, Action<object>? configureOptions = null)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        var pipeline = _defaultPipeline;
        if (configureOptions != null)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter();

            configureOptions(pipelineBuilder);
            pipeline = pipelineBuilder.Build();
        }

        return Markdig.Markdown.ToHtml(markdown, pipeline);
    }

    /// <inheritdoc/>
    public Task<string> RenderToHtmlAsync(string markdown, Action<object>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        // Markdig doesn't have native async rendering, so we wrap the synchronous method
        return Task.FromResult(RenderToHtml(markdown, configureOptions));
    }
}