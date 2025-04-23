using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownPreview(IJSRuntime JSRuntime)
{
    [Parameter]
    public string Markdown { get; set; } = string.Empty;

    [Parameter]
    public string Title { get; set; } = "Preview";

    [Parameter]
    public bool ShowHeader { get; set; } = true;

    [Parameter]
    public bool SyncScroll { get; set; } = true;

    [Parameter]
    public EventCallback<double> OnScroll { get; set; }

    [Parameter]
    public MarkdownPipeline Pipeline { get; set; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();

    /// <summary>
    /// Gets or sets custom CSS classes to apply to the rendered HTML content
    /// </summary>
    [Parameter]
    public string ContentCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets the rendered HTML
    /// </summary>
    public string RenderedHtml => RenderMarkdown(Markdown);

    /// <summary>
    /// Exposes scroll position information to parent components
    /// </summary>
    public double ScrollPosition => _scrollPosition;

    /// <summary>
    /// Sets scroll position programmatically (for sync scrolling)
    /// </summary>
    /// <param name="position">Scroll position as percentage (0-1)</param>
    /// <returns>Task representing the operation</returns>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!SyncScroll || _isScrolling) return;

        try
        {
            _isScrolling = true;

            if (OperatingSystem.IsBrowser() && PreviewContentRef is not null)
            {
                await JSRuntime.InvokeVoidAsync("setScrollPosition", PreviewContentRef, position);
            }
        }
        finally
        {
            _isScrolling = false;
        }
    }

    private ElementReference? PreviewContentRef;
    private double _scrollPosition;
    private bool _isScrolling = false;
    private DotNetObjectReference<MarkdownPreview>? _dotNetRef;
    private string _cachedMarkdown = string.Empty;
    private string _cachedHtml = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        // Only re-render markdown if it has changed
        if (_cachedMarkdown != Markdown)
        {
            _cachedMarkdown = Markdown;
            _cachedHtml = RenderMarkdown(Markdown);

            // After markdown changes, we need to re-apply synced scrolling
            await Task.Delay(50); // Short delay to ensure rendering is complete

            if (SyncScroll && !_isScrolling && OperatingSystem.IsBrowser())
            {
                await SetScrollPositionAsync(_scrollPosition);
            }
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && OperatingSystem.IsBrowser())
        {
            // Initialize JS functionality
            _dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initializeMarkdownPreview", PreviewContentRef, _dotNetRef);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }

    private async Task OnPreviewScroll()
    {
        if (!SyncScroll || _isScrolling) return;

        try
        {
            _isScrolling = true;

            if (OperatingSystem.IsBrowser() && PreviewContentRef is not null)
            {
                // Calculate scroll position as percentage
                var scrollInfo = await JSRuntime.InvokeAsync<ScrollInfo>("getScrollInfo", PreviewContentRef);
                _scrollPosition = scrollInfo.Position;

                // Notify parent of scroll position change
                if (OnScroll.HasDelegate)
                {
                    await OnScroll.InvokeAsync(_scrollPosition);
                }
            }
        }
        finally
        {
            _isScrolling = false;
        }
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
            // Create pipeline if not provided
            var pipeline = Pipeline ?? new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .Build();

            // Render markdown to HTML
            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }
        catch (Exception ex)
        {
            // Return error message for debugging
            return $"<div class=\"markdown-error\">Error rendering markdown: {ex.Message}</div>";
        }
    }

    private string GetPreviewClass()
    {
        return $"osirion-markdown-preview {CssClass}".Trim();
    }

    [JSInvokable]
    public void UpdateScrollPosition(double position)
    {
        _scrollPosition = position;
    }
}
