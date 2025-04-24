using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

/// <summary>
/// A markdown editor component with toolbar and advanced text manipulation
/// </summary>
public partial class MarkdownEditor : OsirionComponentBase, IAsyncDisposable
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
    /// Whether to auto-focus the editor when initialized
    /// </summary>
    [Parameter]
    public bool AutoFocus { get; set; } = false;

    /// <summary>
    /// Whether to enable scroll position synchronization
    /// </summary>
    [Parameter]
    public bool SyncScroll { get; set; } = true;

    /// <summary>
    /// Event callback when the editor is scrolled (position from 0-1)
    /// </summary>
    [Parameter]
    public EventCallback<double> OnScroll { get; set; }

    /// <summary>
    /// Whether to show the markdown toolbar
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    // References to DOM elements
    private ElementReference _editorRef;

    // Track if component has been initialized to avoid JS calls during SSR
    private bool _isInitialized = false;

    // DotNetObjectReference for JS callbacks
    private DotNetObjectReference<MarkdownEditor>? _dotNetRef;

    /// <summary>
    /// Called when the component is initialized
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && IsInteractive)
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            // Only run client-side initialization when in browser
            if (OperatingSystem.IsBrowser())
            {
                // Minimal inline JS - necessary for editor initialization
                await JSRuntime.InvokeVoidAsync("eval", @"
                    const editor = document.getElementById('" + _editorRef.Id + @"');
                    if (editor) {
                        editor.addEventListener('scroll', () => {
                            const scrollTop = editor.scrollTop;
                            const scrollHeight = editor.scrollHeight;
                            const clientHeight = editor.clientHeight;
                            const position = scrollHeight > clientHeight ? 
                                scrollTop / (scrollHeight - clientHeight) : 0;
                            " + _dotNetRef.Value + @".invokeMethodAsync('UpdateScrollPosition', position);
                        });
                    }
                ");

                // Set focus if auto-focus is enabled
                if (AutoFocus)
                {
                    await FocusAsync();
                }
            }

            _isInitialized = true;
        }
    }

    /// <summary>
    /// Updates the scroll position of the editor
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
    /// Sets the editor's scroll position
    /// </summary>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!_isInitialized || !IsInteractive) return;

        await JSRuntime.InvokeVoidAsync("eval", @"
            const editor = document.getElementById('" + _editorRef.Id + @"');
            if (editor) {
                const scrollHeight = editor.scrollHeight;
                const clientHeight = editor.clientHeight;
                if (scrollHeight > clientHeight) {
                    editor.scrollTop = position * (scrollHeight - clientHeight);
                }
            }
        ");
    }

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusAsync()
    {
        if (!_isInitialized || !IsInteractive) return;

        await JSRuntime.InvokeVoidAsync("eval", @"
            const editor = document.getElementById('" + _editorRef.Id + @"');
            if (editor) {
                editor.focus();
            }
        ");
    }

    /// <summary>
    /// Inserts text at the current cursor position
    /// </summary>
    public async Task InsertTextAsync(string text)
    {
        if (!_isInitialized || !IsInteractive) return;

        await JSRuntime.InvokeVoidAsync("eval", @"
            const editor = document.getElementById('" + _editorRef.Id + @"');
            if (editor) {
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                const before = editor.value.substring(0, start);
                const after = editor.value.substring(end);
                
                editor.value = before + " + JsonEncodedText(text) + @" + after;
                const newCursorPos = start + " + text.Length + @";
                editor.selectionStart = newCursorPos;
                editor.selectionEnd = newCursorPos;
                editor.focus();
                
                // Trigger change event to update binding
                const event = new Event('input', { bubbles: true });
                editor.dispatchEvent(event);
            }
        ");
    }

    /// <summary>
    /// Wraps selected text with prefix and suffix, or inserts default text if no selection
    /// </summary>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText)
    {
        if (!_isInitialized || !IsInteractive) return;

        await JSRuntime.InvokeVoidAsync("eval", @"
            const editor = document.getElementById('" + _editorRef.Id + @"');
            if (editor) {
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                const selectedText = editor.value.substring(start, end);
                const before = editor.value.substring(0, start);
                const after = editor.value.substring(end);
                
                // Use selected text or default text if no selection
                const textToWrap = selectedText.length > 0 ? selectedText : " + JsonEncodedText(defaultText) + @";
                
                editor.value = before + " + JsonEncodedText(prefix) + @" + textToWrap + " + JsonEncodedText(suffix) + @" + after;
                
                // Set cursor position appropriately
                if (selectedText.length > 0) {
                    // Select the wrapped text
                    editor.selectionStart = start + " + prefix.Length + @";
                    editor.selectionEnd = start + " + prefix.Length + @" + textToWrap.length;
                } else {
                    // Place cursor after default text
                    const newPosition = start + " + (prefix.Length + defaultText.Length) + @";
                    editor.selectionStart = newPosition;
                    editor.selectionEnd = newPosition;
                }
                
                editor.focus();
                
                // Trigger change event to update binding
                const event = new Event('input', { bubbles: true });
                editor.dispatchEvent(event);
            }
        ");
    }

    /// <summary>
    /// Returns information about the current selection in the editor
    /// </summary>
    public async Task<TextSelection> GetSelectionAsync()
    {
        if (!_isInitialized || !IsInteractive)
            return new TextSelection { Text = string.Empty, Start = 0, End = 0 };

        return await JSRuntime.InvokeAsync<TextSelection>("eval", @"
            const editor = document.getElementById('" + _editorRef.Id + @"');
            if (editor) {
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                const text = editor.value.substring(start, end);
                return { text, start, end };
            }
            return { text: '', start: 0, end: 0 };
        ");
    }

    /// <summary>
    /// Handles special key press events
    /// </summary>
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        // Handle tab key for indentation
        if (e.Key == "Tab" && IsInteractive)
        {
            await JSRuntime.InvokeVoidAsync("eval", @"
                const editor = document.getElementById('" + _editorRef.Id + @"');
                if (editor) {
                    const start = editor.selectionStart;
                    const end = editor.selectionEnd;
                    
                    // If selection spans multiple lines
                    if (start !== end) {
                        const selectedText = editor.value.substring(start, end);
                        
                        // Check if selection contains newlines
                        if (selectedText.indexOf('\n') !== -1) {
                            const before = editor.value.substring(0, start);
                            const after = editor.value.substring(end);
                            
                            let newText;
                            if (" + e.ShiftKey.ToString().ToLowerInvariant() + @") {
                                // Remove tab or 2 spaces from beginning of each line
                                newText = selectedText.replace(/^(\t|  )/gm, '');
                            } else {
                                // Add tab to beginning of each line
                                newText = selectedText.replace(/^/gm, '\t');
                            }
                            
                            editor.value = before + newText + after;
                            
                            // Update selection to cover new text
                            editor.selectionStart = start;
                            editor.selectionEnd = start + newText.length;
                        }
                    } else if (!" + e.ShiftKey.ToString().ToLowerInvariant() + @") {
                        // Insert tab character
                        const before = editor.value.substring(0, start);
                        const after = editor.value.substring(end);
                        
                        editor.value = before + '\t' + after;
                        
                        // Move cursor position
                        editor.selectionStart = editor.selectionEnd = start + 1;
                    }
                    
                    // Trigger change event to update binding
                    const event = new Event('input', { bubbles: true });
                    editor.dispatchEvent(event);
                    
                    // Prevent default tab behavior
                    event.preventDefault();
                }
            ");
        }
    }

    /// <summary>
    /// Handles content changes from the textarea
    /// </summary>
    private async Task HandleContentChanged(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;

        if (Content != newValue)
        {
            Content = newValue;

            if (ContentChanged.HasDelegate)
            {
                await ContentChanged.InvokeAsync(Content);
            }
        }
    }

    /// <summary>
    /// Encodes text for use in JavaScript
    /// </summary>
    private string JsonEncodedText(string text)
    {
        return System.Text.Json.JsonSerializer.Serialize(text);
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();

        // Clean up event listeners
        if (_isInitialized && IsInteractive)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("eval", @"
                    const editor = document.getElementById('" + _editorRef.Id + @"');
                    if (editor) {
                        editor.replaceWith(editor.cloneNode(true));
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