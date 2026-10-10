using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Cms;

/// <summary>Defines the MarkdownPreview type.</summary>
public partial class MarkdownPreview : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IMarkdownRendererService MarkdownRenderer { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private ILogger<MarkdownPreview> Logger { get; set; } = default!;

    /// <summary>Gets or sets the markdown source.</summary>
    [Parameter] public string Markdown { get; set; } = string.Empty;
    /// <summary>Gets or sets the preview title.</summary>
    [Parameter] public string Title { get; set; } = "Preview";
    /// <summary>Gets or sets a value indicating whether the header is shown.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;
    /// <summary>Gets or sets the empty-state placeholder.</summary>
    [Parameter] public string Placeholder { get; set; } = "No content to preview";
    /// <summary>Gets or sets a value indicating whether scrolling is synchronized.</summary>
    [Parameter] public bool SyncScroll { get; set; } = true;
    /// <summary>Gets or sets the CSS class for the content.</summary>
    [Parameter] public string ContentCssClass { get; set; } = string.Empty;
    /// <summary>Gets or sets the callback invoked when the preview scrolls.</summary>
    [Parameter] public EventCallback<double> OnScroll { get; set; }
    /// <summary>Gets or sets the markdown pipeline.</summary>
    [Parameter] public MarkdownPipeline? Pipeline { get; set; }

    private ElementReference PreviewContainer;
    private DotNetObjectReference<MarkdownPreview>? _dotNetReference;
    private bool _preventScrollEvent = false;
    private string? _renderedMarkdown;
    private MarkdownPipeline? _renderedPipeline;

    /// <summary>
    /// Gets the rendered HTML content from the markdown, sanitized with the registered
    /// <see cref="IHtmlContentSanitizer"/> (or the default allow-list when none is registered).
    /// </summary>
    /// <remarks>
    /// The markdown is rendered again only when <see cref="Markdown"/> or <see cref="Pipeline"/> changes,
    /// not on every render of the component, such as the one that follows each scroll event.
    /// </remarks>
    public string RenderedHtml { get; private set; } = string.Empty;

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    /// <summary>Renders the markdown when it or the pipeline changed since the last render.</summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_renderedMarkdown is not null
            && string.Equals(_renderedMarkdown, Markdown, StringComparison.Ordinal)
            && ReferenceEquals(_renderedPipeline, Pipeline))
        {
            return;
        }

        _renderedMarkdown = Markdown;
        _renderedPipeline = Pipeline;
        RenderedHtml = RenderMarkdown(Markdown);
    }

    /// <summary>Performs the OnAfterRender operation asynchronously.</summary>
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
        return $"osirion-markdown-preview {Class}".Trim();
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
            var html = Pipeline is not null
                ? Markdig.Markdown.ToHtml(markdown, Pipeline)
                : MarkdownRenderer.RenderToHtml(markdown);

            return ServiceProvider.GetHtmlContentSanitizer().Sanitize(html);
        }
        catch (Exception ex)
        {
            // The message can carry internal details such as paths or configuration, so it is logged, not shown.
            Logger.LogError(ex, "Failed to render the markdown preview");
            return "<div class=\"markdown-error\">Error rendering markdown.</div>";
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
