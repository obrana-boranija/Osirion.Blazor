using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Osirion.Blazor.Cms;

public partial class MarkdownEditor : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public string Content { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ContentChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Enter markdown here...";
    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public string Title { get; set; } = "Editor";
    [Parameter] public bool AutoFocus { get; set; } = false;
    [Parameter] public bool SyncScroll { get; set; } = true;
    [Parameter] public EventCallback<double> OnScroll { get; set; }
    [Parameter] public List<ToolbarAction>? ToolbarActions { get; set; }

    private ElementReference TextAreaRef;
    private ElementReference EditorContainer;
    private string _currentContent = string.Empty;
    private bool _preventScrollEvent = false;
    private DotNetObjectReference<MarkdownEditor>? _dotNetReference;
    private List<ToolbarAction> _defaultToolbarActions;

    public MarkdownEditor()
    {
        // Initialize default toolbar actions in constructor
        _defaultToolbarActions = new List<ToolbarAction>
        {
            new ToolbarAction("H", ToolbarActionType.Wrap, "## ||Heading", "Heading",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M4 12h16\"/><path d=\"M4 18h12\"/><path d=\"M4 6h16\"/></svg>"),

            new ToolbarAction("B", ToolbarActionType.Wrap, "**|**|bold text", "Bold",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M6 4h8a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/><path d=\"M6 12h9a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/></svg>"),

            new ToolbarAction("I", ToolbarActionType.Wrap, "*|*|italic text", "Italic",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><line x1=\"19\" y1=\"4\" x2=\"10\" y2=\"4\"/><line x1=\"14\" y1=\"20\" x2=\"5\" y2=\"20\"/><line x1=\"15\" y1=\"4\" x2=\"9\" y2=\"20\"/></svg>"),

            new ToolbarAction("Link", ToolbarActionType.Wrap, "[|](url)|link text", "Link",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71\"/><path d=\"M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71\"/></svg>"),

            new ToolbarAction("List", ToolbarActionType.Insert, "\n- Item 1\n- Item 2\n- Item 3", "Bullet List",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><line x1=\"8\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"8\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"8\" y1=\"18\" x2=\"21\" y2=\"18\"/><line x1=\"3\" y1=\"6\" x2=\"3.01\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"3.01\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"3.01\" y2=\"18\"/></svg>"),

            new ToolbarAction("Code", ToolbarActionType.Wrap, "```\n|\n```|code goes here", "Code Block",
                "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><polyline points=\"16 18 22 12 16 6\"/><polyline points=\"8 6 2 12 8 18\"/></svg>")
        };
    }

    /// <summary>
    /// Gets or sets the current content with two-way binding to update the parent
    /// </summary>
    private string CurrentContent
    {
        get => _currentContent;
        set
        {
            if (_currentContent != value)
            {
                _currentContent = value;
                _ = NotifyContentChangedAsync(value);
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _currentContent = Content;

        // Use default toolbar actions if none provided
        if (ToolbarActions is null)
        {
            ToolbarActions = _defaultToolbarActions;
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Update local content if external content changes
        if (Content != _currentContent)
        {
            _currentContent = Content;
        }

        // Ensure ToolbarActions is never null
        ToolbarActions ??= _defaultToolbarActions;

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Create reference for JS interop
            _dotNetReference = DotNetObjectReference.Create(this);

            // Auto focus if configured
            if (AutoFocus)
            {
                await FocusAsync();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Handle key down events, especially tab key
    /// </summary>
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        // Handle tab key to insert indentation instead of changing focus
        if (e.Key == "Tab")
        {
            await HandleTabKeyAsync(e.ShiftKey);
        }
    }

    /// <summary>
    /// Handle scroll events and notify parent for sync
    /// </summary>
    private async Task HandleScroll()
    {
        if (_preventScrollEvent || !SyncScroll || !OnScroll.HasDelegate)
            return;

        // Use minimal inline JS to get scroll position
        var position = await GetScrollPositionAsync();
        await OnScroll.InvokeAsync(position);
    }

    /// <summary>
    /// Execute toolbar action
    /// </summary>
    private async Task ExecuteToolbarAction(ToolbarAction action)
    {
        if (action.Action is not null)
        {
            switch (action.ActionType)
            {
                case ToolbarActionType.Insert:
                    await InsertTextAsync(action.Action);
                    break;
                case ToolbarActionType.Wrap:
                    var parameters = action.Action.Split('|');
                    if (parameters.Length >= 3)
                    {
                        await WrapTextAsync(parameters[0], parameters[1], parameters[2]);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Notifies the parent component when content changes
    /// </summary>
    private async Task NotifyContentChangedAsync(string value)
    {
        if (ContentChanged.HasDelegate && value != Content)
        {
            await ContentChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Gets the CSS class for the component
    /// </summary>
    private string GetCssClass()
    {
        return $"osirion-markdown-editor {Class}".Trim();
    }

    #region JavaScript Interop Methods

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("eval",
                $"document.querySelector('[_bl_{TextAreaRef.Id}]')?.focus()");
        }
        catch (Exception)
        {
            // Ignore focus errors in SSR
        }
    }

    // Update the InsertTextAsync method
    public async Task InsertTextAsync(string text)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const textarea = document.querySelector('[_bl_{TextAreaRef.Id}]');
                    if (!textarea) return;
                    
                    const start = textarea.selectionStart;
                    const end = textarea.selectionEnd;
                    const before = textarea.value.substring(0, start);
                    const after = textarea.value.substring(end);
                    
                    // Set new text and update cursor position
                    textarea.value = before + {JsonSerializer.Serialize(text)} + after;
                    
                    // Set selection after inserted text
                    const newCursorPos = start + {text.Length};
                    textarea.selectionStart = newCursorPos;
                    textarea.selectionEnd = newCursorPos;
                    
                    // Focus the textarea
                    textarea.focus();
                    
                    // Trigger input event to update Blazor binding
                    textarea.dispatchEvent(new Event('input'));
                }})()");
        }
        catch
        {
            // Fallback if JS interop fails
            CurrentContent += text;
        }
    }

    // Update the WrapTextAsync method
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText)
    {
        try
        {
            var serializedPrefix = JsonSerializer.Serialize(prefix); // Serialize prefix
            var serializedSuffix = JsonSerializer.Serialize(suffix); // Serialize suffix
            var serializedDefaultText = JsonSerializer.Serialize(defaultText); // Serialize defaultText

            await JSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const textarea = document.querySelector('[_bl_{TextAreaRef.Id}]');
                    if (!textarea) return;
                    
                    const start = textarea.selectionStart;
                    const end = textarea.selectionEnd;
                    const selectedText = textarea.value.substring(start, end);
                    const before = textarea.value.substring(0, start);
                    const after = textarea.value.substring(end);
                    
                    // Use selected text or default text if no selection
                    const textToWrap = selectedText.length > 0 ? selectedText : {serializedDefaultText};
                    
                    // Set new text with wrapping
                    textarea.value = before + {serializedPrefix} + textToWrap + {serializedSuffix} + after;
                    
                    // Set cursor position and selection
                    if (selectedText.length > 0) {{
                        // Select the wrapped text
                        textarea.selectionStart = start + {prefix.Length};
                        textarea.selectionEnd = start + {prefix.Length} + textToWrap.length;
                    }} else {{
                        // Place cursor after the default text
                        const newPosition = start + {prefix.Length} + {defaultText.Length};
                        textarea.selectionStart = newPosition;
                        textarea.selectionEnd = newPosition;
                    }}
                    
                    // Focus the textarea
                    textarea.focus();
                    
                    // Trigger input event to update Blazor binding
                    textarea.dispatchEvent(new Event('input'));
                }})()");
        }
        catch
        {
            // Fallback if JS interop fails
            CurrentContent += $"{prefix}{defaultText}{suffix}";
        }
    }

    /// <summary>
    /// Handles tab key in textarea for indentation
    /// </summary>
    private async Task HandleTabKeyAsync(bool isShiftKey)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const textarea = document.querySelector('[_bl_{TextAreaRef.Id}]');
                    if (!textarea) return;
                    
                    const start = textarea.selectionStart;
                    const end = textarea.selectionEnd;
                    
                    // If selection spans multiple lines
                    if (start !== end) {{
                        const selectedText = textarea.value.substring(start, end);
                        
                        // Check if selection contains newlines
                        if (selectedText.indexOf('\n') !== -1) {{
                            const before = textarea.value.substring(0, start);
                            const after = textarea.value.substring(end);
                            
                            let newText;
                            
                            if ({isShiftKey.ToString().ToLowerInvariant()}) {{
                                // Remove tab or 2 spaces from the beginning of each line
                                newText = selectedText.replace(/^(\t|  )/gm, '');
                            }} else {{
                                // Add tab to the beginning of each line
                                newText = selectedText.replace(/^/gm, '\t');
                            }}
                            
                            textarea.value = before + newText + after;
                            
                            // Update selection to cover the new text
                            textarea.selectionStart = start;
                            textarea.selectionEnd = start + newText.length;
                        }}
                    }} else if (!{isShiftKey.ToString().ToLowerInvariant()}) {{
                        // Single line or no selection - insert tab character
                        const before = textarea.value.substring(0, start);
                        const after = textarea.value.substring(end);
                        
                        textarea.value = before + '\t' + after;
                        
                        // Move cursor position
                        textarea.selectionStart = textarea.selectionEnd = start + 1;
                    }}
                    
                    // Trigger input event to update Blazor binding
                    textarea.dispatchEvent(new Event('input'));
                    
                    // Prevent default tab behavior
                    event.preventDefault();
                }})()");
        }
        catch
        {
            // Fallback - just add a tab
            CurrentContent += "\t";
        }
    }

    /// <summary>
    /// Gets the scroll position of the editor
    /// </summary>
    private async Task<double> GetScrollPositionAsync()
    {
        try
        {
            return await JSRuntime.InvokeAsync<double>("eval", $@"
                (function() {{
                    const element = document.querySelector('[_bl_{EditorContainer.Id}]');
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
    /// Sets the scroll position of the editor
    /// </summary>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!SyncScroll) return;

        try
        {
            _preventScrollEvent = true;

            await JSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const element = document.querySelector('[_bl_{EditorContainer.Id}]');
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

    #endregion

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

/// <summary>
/// Represents a button in the Markdown editor toolbar
/// </summary>
public class ToolbarAction
{
    /// <summary>
    /// Text label for the button
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Type of action to perform
    /// </summary>
    public ToolbarActionType ActionType { get; set; }

    /// <summary>
    /// Action data - interpretation depends on ActionType
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Title/tooltip for the button
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Optional icon HTML
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Creates a new toolbar action
    /// </summary>
    /// <param name="label"></param>
    /// <param name="actionType"></param>
    /// <param name="action"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public ToolbarAction(string label, ToolbarActionType actionType, string? action, string title, string? icon = null)
    {
        Label = label;
        ActionType = actionType;
        Action = action;
        Title = title;
        Icon = icon;
    }
}

/// <summary>
/// Types of toolbar actions
/// </summary>
public enum ToolbarActionType
{
    /// <summary>
    /// Insert text at cursor position
    /// </summary>
    Insert,

    /// <summary>
    /// Wrap selected text with prefix and suffix
    /// Format: "prefix|suffix|defaultText"
    /// </summary>
    Wrap,

    /// <summary>
    /// Custom action to be handled by the consumer
    /// </summary>
    Custom
}