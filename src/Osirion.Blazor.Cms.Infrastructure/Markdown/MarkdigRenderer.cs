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

    /// <summary>Performs the MarkdigRenderer operation.</summary>
    public MarkdigRenderer()
    {
        _defaultPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    /// <summary>Performs the ObjectRenderers operation.</summary>
    public ObjectRendererCollection ObjectRenderers => throw new NotImplementedException();

#pragma warning disable CS0067 // Events are required by the renderer contract and used when object rendering is implemented.
    /// <summary>Performs the ObjectWriteBefore operation.</summary>
    public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore = delegate { };
    /// <summary>Performs the ObjectWriteAfter operation.</summary>
    public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter = delegate { };
#pragma warning restore CS0067

    /// <summary>Performs the Render operation.</summary>
    public object Render(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string RenderToHtml(string markdown, Action<object>? configureOptions = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var pipeline = _defaultPipeline;
        if (configureOptions is not null)
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
