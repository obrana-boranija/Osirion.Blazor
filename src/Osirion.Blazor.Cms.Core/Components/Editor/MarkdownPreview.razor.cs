using Markdig;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Services;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownPreview
{
    [Parameter]
    public string Markdown { get; set; } = string.Empty;

    [Parameter]
    public string Title { get; set; } = "Preview";

    [Parameter]
    public bool ShowHeader { get; set; } = true;

    [Parameter]
    public string Placeholder { get; set; } = "No content to preview";

    [Parameter]
    public MarkdownPipeline Pipeline { get; set; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();

    [Inject] 
    IMarkdownRendererService MarkdownRenderer { get; set; } = default!;

    /// <summary>
    /// Gets the rendered HTML
    /// </summary>
    public string RenderedHtml => RenderMarkdown(Markdown);

    private string GetCssClass()
    {
        return $"osirion-markdown-preview {CssClass}".Trim();
    }

    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    private string RenderMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        try
        {
            return MarkdownRenderer.RenderToHtml(markdown);

        }
        catch (Exception ex)
        {
            // Return error message for debugging
            return $"<div class=\"markdown-error\">Error rendering markdown: {ex.Message}</div>";
        }
    }
}