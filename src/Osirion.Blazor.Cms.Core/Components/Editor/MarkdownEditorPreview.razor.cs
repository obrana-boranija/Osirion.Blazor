using Markdig;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownEditorPreview
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Enter markdown content here...";

    [Parameter]
    public string EditorTitle { get; set; } = "Editor";

    [Parameter]
    public string PreviewTitle { get; set; } = "Preview";

    [Parameter]
    public bool AutoFocus { get; set; } = false;

    [Parameter]
    public bool SyncScroll { get; set; } = true;

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    [Parameter]
    public EventCallback<bool> ShowPreviewChanged { get; set; }

    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    [Parameter]
    public bool ShowPreviewHeader { get; set; } = true;

    [Parameter]
    public string EditorCssClass { get; set; } = string.Empty;

    [Parameter]
    public string PreviewCssClass { get; set; } = string.Empty;

    [Parameter]
    public string PreviewContentCssClass { get; set; } = string.Empty;

    [Parameter]
    public MarkdownPipeline Pipeline { get; set; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();

    private MarkdownEditor EditorRef { get; set; } = default!;
    private MarkdownPreview PreviewRef { get; set; } = default!;
    private bool _isProcessingScroll = false;

    /// <summary>
    /// Gets the markdown editor component reference
    /// </summary>
    public MarkdownEditor Editor => EditorRef;

    /// <summary>
    /// Gets the markdown preview component reference
    /// </summary>
    public MarkdownPreview Preview => PreviewRef;

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusEditorAsync()
    {
        if (EditorRef != null)
        {
            await EditorRef.FocusAsync();
        }
    }

    /// <summary>
    /// Inserts text at the current cursor position
    /// </summary>
    /// <param name="text">Text to insert</param>
    /// <returns>Task representing the operation</returns>
    public async Task InsertTextAsync(string text)
    {
        if (EditorRef != null)
        {
            await EditorRef.InsertTextAsync(text);
        }
    }

    /// <summary>
    /// Wraps selected text with prefix and suffix
    /// </summary>
    /// <param name="prefix">Text to add before selection</param>
    /// <param name="suffix">Text to add after selection</param>
    /// <param name="defaultText">Default text if no selection</param>
    /// <returns>Task representing the operation</returns>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText = "")
    {
        if (EditorRef != null)
        {
            await EditorRef.WrapTextAsync(prefix, suffix, defaultText);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private async Task HandleContentChanged(string newContent)
    {
        Content = newContent;

        if (ContentChanged.HasDelegate)
        {
            await ContentChanged.InvokeAsync(newContent);
        }
    }

    private async Task HandleEditorScroll(double position)
    {
        if (!SyncScroll || _isProcessingScroll || PreviewRef == null) return;

        try
        {
            _isProcessingScroll = true;
            await PreviewRef.SetScrollPositionAsync(position);
        }
        finally
        {
            _isProcessingScroll = false;
        }
    }

    private async Task HandlePreviewScroll(double position)
    {
        if (!SyncScroll || _isProcessingScroll || EditorRef == null) return;

        try
        {
            _isProcessingScroll = true;
            await EditorRef.SetScrollPositionAsync(position);
        }
        finally
        {
            _isProcessingScroll = false;
        }
    }

    private async Task TogglePreview()
    {
        ShowPreview = !ShowPreview;

        if (ShowPreviewChanged.HasDelegate)
        {
            await ShowPreviewChanged.InvokeAsync(ShowPreview);
        }
    }

    private string GetLayoutClass()
    {
        return ShowPreview ? "osirion-split-layout" : "osirion-editor-only-layout";
    }

    private string GetEditorPreviewClass()
    {
        return $"osirion-markdown-editor-preview {CssClass}".Trim();
    }
}
