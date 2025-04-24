using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Services;

public interface IMarkdownJsLoader
{
    Task EnsureInitializedAsync();
    bool IsInitialized { get; }
}

public class MarkdownJsLoader : IMarkdownJsLoader
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<MarkdownJsLoader> _logger;
    private bool _isInitialized = false;
    private readonly object _lock = new object();
    private Task _initializationTask = null;

    public bool IsInitialized => _isInitialized;

    public MarkdownJsLoader(IJSRuntime jsRuntime, ILogger<MarkdownJsLoader> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public Task EnsureInitializedAsync()
    {
        if (_isInitialized)
            return Task.CompletedTask;

        lock (_lock)
        {
            if (_initializationTask == null)
            {
                _initializationTask = InitializeAsyncInternal();
            }
        }

        return _initializationTask;
    }

    private async Task InitializeAsyncInternal()
    {
        try
        {
            // First try to check if it's already loaded
            var alreadyLoaded = await CheckIfAlreadyLoadedAsync();
            if (alreadyLoaded)
            {
                _isInitialized = true;
                _logger.LogInformation("Markdown Editor JavaScript is already initialized");
                return;
            }

            // Load the script dynamically
            await LoadScriptAsync();

            // Direct inline JavaScript as a fallback
            await RegisterFunctionsAsync();

            _isInitialized = true;
            _logger.LogInformation("Markdown Editor JavaScript initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Markdown Editor JavaScript");
            // Don't throw - allow the application to continue even if JS fails
        }
    }

    private async Task<bool> CheckIfAlreadyLoadedAsync()
    {
        try
        {
            // Check if one of the core functions exists
            return await _jsRuntime.InvokeAsync<bool>("eval",
                "typeof window.initializeMarkdownEditor === 'function'");
        }
        catch
        {
            return false;
        }
    }

    private async Task LoadScriptAsync()
    {
        await _jsRuntime.InvokeVoidAsync("eval", @"
        const script = document.createElement('script');
        script.src = '_content/Osirion.Blazor.Cms.Core/js/markdownEditorBridge.js';
        script.type = 'module';
        script.onload = function() { 
            console.log('Markdown Editor bridge loaded successfully'); 
        };
        document.head.appendChild(script);
    ");
    }

    private async Task RegisterFunctionsAsync()
    {
        // Define all the functions inline as a fallback
        // This ensures the functions are available even if the script fails to load
        await _jsRuntime.InvokeVoidAsync("eval", @"
                if (typeof window.initializeMarkdownEditor !== 'function') {
                    console.log('Defining Markdown Editor functions inline');
                    
                    window.initializeMarkdownEditor = function(editorElement, dotNetRef) {
                        if (!editorElement) return;
                        editorElement.addEventListener('scroll', function() {
                            const position = window.calculateScrollPosition(editorElement);
                            dotNetRef.invokeMethodAsync('UpdateScrollPosition', position);
                        });
                    };

                    window.initializeMarkdownPreview = function(previewElement, dotNetRef) {
                        if (!previewElement) return;
                        previewElement.addEventListener('scroll', function() {
                            const position = window.calculateScrollPosition(previewElement);
                            dotNetRef.invokeMethodAsync('UpdateScrollPosition', position);
                        });
                    };

                    window.calculateScrollPosition = function(element) {
                        if (!element) return 0;
                        const scrollTop = element.scrollTop;
                        const scrollHeight = element.scrollHeight;
                        const clientHeight = element.clientHeight;
                        let position = 0;
                        if (scrollHeight > clientHeight) {
                            position = scrollTop / (scrollHeight - clientHeight);
                        }
                        return position;
                    };

                    window.getScrollInfo = function(element) {
                        if (!element) return { position: 0, scrollTop: 0, scrollHeight: 0, clientHeight: 0 };
                        const scrollTop = element.scrollTop;
                        const scrollHeight = element.scrollHeight;
                        const clientHeight = element.clientHeight;
                        let position = 0;
                        if (scrollHeight > clientHeight) {
                            position = scrollTop / (scrollHeight - clientHeight);
                        }
                        return { position, scrollTop, scrollHeight, clientHeight };
                    };

                    window.setScrollPosition = function(element, position) {
                        if (!element) return;
                        const scrollHeight = element.scrollHeight;
                        const clientHeight = element.clientHeight;
                        if (scrollHeight <= clientHeight) return;
                        const targetScrollTop = position * (scrollHeight - clientHeight);
                        element.scrollTop = targetScrollTop;
                    };

                    window.getTextAreaSelection = function(textarea) {
                        if (!textarea) return { text: '', start: 0, end: 0 };
                        const start = textarea.selectionStart;
                        const end = textarea.selectionEnd;
                        const text = textarea.value.substring(start, end);
                        return { text, start, end };
                    };

                    window.insertTextAtCursor = function(textarea, text) {
                        if (!textarea) return;
                        const start = textarea.selectionStart;
                        const end = textarea.selectionEnd;
                        const before = textarea.value.substring(0, start);
                        const after = textarea.value.substring(end);
                        textarea.value = before + text + after;
                        const newCursorPos = start + text.length;
                        textarea.selectionStart = newCursorPos;
                        textarea.selectionEnd = newCursorPos;
                        textarea.focus();
                    };

                    window.wrapTextSelection = function(textarea, prefix, suffix, defaultText) {
                        if (!textarea) return;
                        const start = textarea.selectionStart;
                        const end = textarea.selectionEnd;
                        const selectedText = textarea.value.substring(start, end);
                        const before = textarea.value.substring(0, start);
                        const after = textarea.value.substring(end);
                        const textToWrap = selectedText.length > 0 ? selectedText : defaultText;
                        textarea.value = before + prefix + textToWrap + suffix + after;
                        if (selectedText.length > 0) {
                            textarea.selectionStart = start + prefix.length;
                            textarea.selectionEnd = start + prefix.length + textToWrap.length;
                        } else {
                            const newPosition = start + prefix.length + defaultText.length;
                            textarea.selectionStart = newPosition;
                            textarea.selectionEnd = newPosition;
                        }
                        textarea.focus();
                    };

                    window.handleTabKey = function(textarea, isShiftKey) {
                        if (!textarea) return;
                        const start = textarea.selectionStart;
                        const end = textarea.selectionEnd;
                        if (start !== end) {
                            const selectedText = textarea.value.substring(start, end);
                            if (selectedText.indexOf('\n') !== -1) {
                                const before = textarea.value.substring(0, start);
                                const after = textarea.value.substring(end);
                                let newText;
                                if (isShiftKey) {
                                    newText = selectedText.replace(/^(\t|  )/gm, '');
                                } else {
                                    newText = selectedText.replace(/^/gm, '\t');
                                }
                                textarea.value = before + newText + after;
                                textarea.selectionStart = start;
                                textarea.selectionEnd = start + newText.length;
                                return;
                            }
                        }
                        if (!isShiftKey) {
                            const before = textarea.value.substring(0, start);
                            const after = textarea.value.substring(end);
                            textarea.value = before + '\t' + after;
                            textarea.selectionStart = textarea.selectionEnd = start + 1;
                        }
                    };

                    window.getElementValue = function(element) {
                        return element ? element.value : '';
                    };

                    window.focusElement = function(element) {
                        if (element && typeof element.focus === 'function') {
                            element.focus();
                        }
                    };

                    console.log('Markdown Editor functions defined successfully');
                }
            ");
    }
}