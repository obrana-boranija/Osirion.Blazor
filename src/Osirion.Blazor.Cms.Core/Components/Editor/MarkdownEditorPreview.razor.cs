using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

/// <summary>
/// A combined markdown editor and preview component with synchronized scrolling
/// </summary>
public partial class MarkdownEditorPreview : OsirionComponentBase
{
    /// <summary>
    /// Gets or sets the markdown content
    /// </summary>
    [Parameter]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Event callback when content changes
    /// </summary>
    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    /// <summary>
    /// Placeholder text when editor is empty
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Enter markdown here...";

    /// <summary>
    /// The title displayed in the editor header
    /// </summary>
    [Parameter]
    public string EditorTitle { get; set; } = "Editor";

    /// <summary>
    /// The title displayed in the preview header
    /// </summary>
    [Parameter]
    public string PreviewTitle { get; set; } = "Preview";

    /// <summary>
    /// Whether to auto-focus the editor
    /// </summary>
    [Parameter]
    public bool AutoFocus { get; set; } = false;

    /// <summary>
    /// Whether to synchronize scrolling between editor and preview
    /// </summary>
    [Parameter]
    public bool SyncScroll { get; set; } = true;

    /// <summary>
    /// Whether to show the preview panel
    /// </summary>
    [Parameter]
    public bool ShowPreview { get; set; } = true;

    /// <summary>
    /// Event callback when preview visibility changes
    /// </summary>
    [Parameter]
    public EventCallback<bool> ShowPreviewChanged { get; set; }

    /// <summary>
    /// Whether to show the markdown toolbar
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Whether to show the preview header
    /// </summary>
    [Parameter]
    public bool ShowPreviewHeader { get; set; } = true;

    /// <summary>
    /// CSS class to apply to the editor
    /// </summary>
    [Parameter]
    public string EditorCssClass { get; set; } = string.Empty;

    /// <summary>
    /// CSS class to apply to the preview
    /// </summary>
    [Parameter]
    public string PreviewCssClass { get; set; } = string.Empty;

    /// <summary>
    /// CSS class to apply to the preview content
    /// </summary>
    [Parameter]
    public string PreviewContentCssClass { get; set; } = string.Empty;

    // References to child components
    private MarkdownEditor? _editorRef;
    private MarkdownPreview? _previewRef;

    // Flag to prevent infinite scroll sync loop
    private bool _isScrolling = false;

    /// <summary>
    /// Toggles the preview panel
    /// </summary>
    public async Task TogglePreviewAsync()
    {
        ShowPreview = !ShowPreview;

        if (ShowPreviewChanged.HasDelegate)
        {
            await ShowPreviewChanged.InvokeAsync(ShowPreview);
        }
    }

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusEditorAsync()
    {
        if (_editorRef != null)
        {
            await _editorRef.FocusAsync();
        }
    }

    /// <summary>
    /// Inserts text at the current cursor position
    /// </summary>
    public async Task InsertTextAsync(string text)
    {
        if (_editorRef != null)
        {
            await _editorRef.InsertTextAsync(text);
        }
    }

    /// <summary>
    /// Wraps selected text with prefix and suffix
    /// </summary>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText)
    {
        if (_editorRef != null)
        {
            await _editorRef.WrapTextAsync(prefix, suffix, defaultText);
        }
    }

    /// <summary>
    /// Handles changes to the markdown content
    /// </summary>
    private async Task HandleContentChanged(string newContent)
    {
        Content = newContent;

        if (ContentChanged.HasDelegate)
        {
            await ContentChanged.InvokeAsync(newContent);
        }
    }

    /// <summary>
    /// Handles scroll events from the editor
    /// </summary>
    private async Task SyncPreviewScroll(double position)
    {
        // Prevent infinite loop of scroll events
        if (!_isScrolling && SyncScroll && ShowPreview && _previewRef != null)
        {
            try
            {
                _isScrolling = true;
                await _previewRef.SetScrollPositionAsync(position);
            }
            finally
            {
                _isScrolling = false;
            }
        }
    }

    /// <summary>
    /// Handles scroll events from the preview
    /// </summary>
    private async Task SyncEditorScroll(double position)
    {
        // Prevent infinite loop of scroll events
        if (!_isScrolling && SyncScroll && _editorRef != null)
        {
            try
            {
                _isScrolling = true;
                await _editorRef.SetScrollPositionAsync(position);
            }
            finally
            {
                _isScrolling = false;
            }
        }
    }

    /// <summary>
    /// Builds the CSS class for the component
    /// </summary>
    private string GetCssClass()
    {
        return $"osirion-markdown-editor-preview {(ShowPreview ? "preview-visible" : "preview-hidden")} {CssClass}".Trim();
    }
}