using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Core.Interfaces;

namespace Osirion.Blazor.Cms;

public partial class MarkdownPreview : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IMarkdownRendererService MarkdownRenderer { get; set; } = default!;

    [Parameter] public string Markdown { get; set; } = string.Empty;
    [Parameter] public string Title { get; set; } = "Preview";
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public string Placeholder { get; set; } = "No content to preview";
    [Parameter] public bool SyncScroll { get; set; } = true;
    [Parameter] public string ContentCssClass { get; set; } = string.Empty;
    [Parameter] public EventCallback<double> OnScroll { get; set; }
    [Parameter] public MarkdownPipeline? Pipeline { get; set; }

    private ElementReference PreviewContainer;
    private DotNetObjectReference<MarkdownPreview>? _dotNetReference;
    private bool _preventScrollEvent = false;

    /// <summary>
    /// Gets the rendered HTML content from the markdown
    /// </summary>
    public string RenderedHtml => RenderMarkdown(Markdown);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetReference = DotNetObjectReference.Create(this);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Gets the CSS class for the component
    /// </summary>
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
            if (Pipeline != null)
            {
                return Markdig.Markdown.ToHtml(markdown, Pipeline);
            }

            return MarkdownRenderer.RenderToHtml(markdown);
        }
        catch (Exception ex)
        {
            return $"<div class=\"markdown-error\">Error rendering markdown: {ex.Message}</div>";
        }
    }

    /// <summary>
    /// Handle scroll events and notify parent for sync
    /// </summary>
    private async Task HandleScroll()
    {
        if (_preventScrollEvent || !SyncScroll || !OnScroll.HasDelegate)
            return;

        var position = await GetScrollPositionAsync();
        await OnScroll.InvokeAsync(position);
    }

    /// <summary>
    /// Gets the scroll position
    /// </summary>
    private async Task<double> GetScrollPositionAsync()
    {
        try
        {
            return await JSRuntime.InvokeAsync<double>("eval", $@"
                (function() {{
                    const element = document.querySelector('[_bl_{PreviewContainer.Id}]');
                    if (!element) return 0;
                    
                    const scrollTop = element.scrollTop;
                    const scrollHeight = element.scrollHeight;
                    const clientHeight = element.clientHeight;
                    
                    // Calculate the scroll percentage (0 to 1)
                    let position = 0;
                    if (scrollHeight > clientHeight) {{
                        position = scrollTop / (scrollHeight - clientHeight);
                    }}
                    
                    return position;
                }})()");
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Sets the scroll position
    /// </summary>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!SyncScroll) return;

        try
        {
            _preventScrollEvent = true;

            await JSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const element = document.querySelector('[_bl_{PreviewContainer.Id}]');
                    if (!element) return;
                    
                    const scrollHeight = element.scrollHeight;
                    const clientHeight = element.clientHeight;
                    
                    if (scrollHeight <= clientHeight) return;
                    
                    // Calculate the target scroll position
                    const targetScrollTop = {position} * (scrollHeight - clientHeight);
                    
                    // Set the scroll position
                    element.scrollTop = targetScrollTop;
                }})()");
        }
        catch
        {
            // Ignore scroll errors in SSR
        }
        finally
        {
            _preventScrollEvent = false;
        }
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            _dotNetReference?.Dispose();
        }
        catch
        {
            // Ignore disposal errors
        }
    }
}