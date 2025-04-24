using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Markdig;
using System.Threading.Tasks;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

/// <summary>
/// A component that renders markdown content as HTML with synchronized scrolling
/// </summary>
public partial class MarkdownPreview : OsirionComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the markdown content to preview
    /// </summary>
    [Parameter]
    public string Markdown { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title displayed in the preview header
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Preview";

    /// <summary>
    /// Gets or sets whether to show the preview header
    /// </summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable scroll position synchronization
    /// </summary>
    [Parameter]
    public bool SyncScroll { get; set; } = true;

    /// <summary>
    /// Gets or sets the Markdig pipeline for rendering markdown
    /// </summary>
    [Parameter]
    public MarkdownPipeline? Pipeline { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text when no content is available
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "No content to preview";

    /// <summary>
    /// Gets or sets the CSS class for the content container
    /// </summary>
    [Parameter]
    public string ContentCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Event callback when the preview is scrolled (position from 0-1)
    /// </summary>
    [Parameter]
    public EventCallback<double> OnScroll { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    // Element reference for the preview container
    private ElementReference _previewRef;

    // DotNetObjectReference for JS callbacks
    private DotNetObjectReference<MarkdownPreview>? _dotNetRef;

    // Track initialization state
    private bool _isInitialized = false;

    /// <summary>
    /// Called after the component has been rendered
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && IsInteractive)
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            // Initialize scroll tracking if we're in the browser
            if (OperatingSystem.IsBrowser())
            {
                await JSRuntime.InvokeVoidAsync("eval", @"
                    const preview = document.getElementById('" + _previewRef.Id + @"');
                    if (preview) {
                        preview.addEventListener('scroll', () => {
                            const scrollTop = preview.scrollTop;
                            const scrollHeight = preview.scrollHeight;
                            const clientHeight = preview.clientHeight;
                            const position = scrollHeight > clientHeight ? 
                                scrollTop / (scrollHeight - clientHeight) : 0;
                            " + _dotNetRef.Value + @".invokeMethodAsync('UpdateScrollPosition', position);
                        });
                    }
                ");
            }

            _isInitialized = true;
        }
    }

    /// <summary>
    /// Updates the scroll position of the preview
    /// </summary>
    [JSInvokable]
    public async Task UpdateScrollPosition(double position)
    {
        if (SyncScroll && OnScroll.HasDelegate)
        {
            await OnScroll.InvokeAsync(position);
        }
    }

    /// <summary>
    /// Sets the preview's scroll position
    /// </summary>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!_isInitialized || !IsInteractive) return;

        await JSRuntime.InvokeVoidAsync("eval", @"
            const preview = document.getElementById('" + _previewRef.Id + @"');
            if (preview) {
                const scrollHeight = preview.scrollHeight;
                const clientHeight = preview.clientHeight;
                if (scrollHeight > clientHeight) {
                    preview.scrollTop = position * (scrollHeight - clientHeight);
                }
            }
        ");
    }

    /// <summary>
    /// Gets the rendered HTML from the markdown content
    /// </summary>
    protected string GetRenderedHtml()
    {
        if (string.IsNullOrWhiteSpace(Markdown))
            return string.Empty;

        try
        {
            if (Pipeline != null)
            {
                return Markdig.Markdown.ToHtml(Markdown, Pipeline);
            }
            else
            {
                // Use default pipeline with basic security
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .DisableHtml() // Prevent raw HTML injection
                    .Build();

                return Markdig.Markdown.ToHtml(Markdown, pipeline);
            }
        }
        catch (Exception ex)
        {
            return $"<div class=\"markdown-error\">Error rendering markdown: {ex.Message}</div>";
        }
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();

        // Clean up scroll event listener
        if (_isInitialized && IsInteractive)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("eval", @"
                    const preview = document.getElementById('" + _previewRef.Id + @"');
                    if (preview) {
                        preview.replaceWith(preview.cloneNode(true));
                    }
                ");
            }
            catch
            {
                // Ignore errors during disposal
            }
        }
    }
}