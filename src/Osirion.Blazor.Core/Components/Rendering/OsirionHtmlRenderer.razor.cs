using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Components;

/// <summary>
/// Component for rendering HTML content with optional syntax highlighting
/// </summary>
public partial class OsirionHtmlRenderer
{
    /// <summary>
    /// The HTML content to render
    /// </summary>
    [Parameter]
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Whether to enable syntax highlighting for code blocks
    /// </summary>
    [Parameter]
    public bool EnableSyntaxHighlighting { get; set; } = true;

    /// <summary>
    /// Whether to show line numbers for code blocks
    /// </summary>
    [Parameter]
    public bool ShowLineNumbers { get; set; } = false;

    /// <summary>
    /// Whether to enable the copy button for code blocks
    /// </summary>
    [Parameter]
    public bool EnableCopyButton { get; set; } = false;

    /// <summary>
    /// Whether to enable line highlighting functionality
    /// </summary>
    [Parameter]
    public bool EnableLineHighlighting { get; set; } = false;

    /// <summary>
    /// Whether to use the WCAG AAA compliant theme
    /// </summary>
    [Parameter]
    public bool UseAccessibleTheme { get; set; } = true;

    /// <summary>
    /// Custom CSS classes for specific languages
    /// </summary>
    [Parameter]
    public Dictionary<string, string>? LanguageClasses { get; set; }

    /// <summary>
    /// Whether to sanitize the HTML content (recommended for untrusted content)
    /// </summary>
    [Parameter]
    public bool SanitizeHtml { get; set; } = false;

    /// <summary>
    /// Callback to provide custom HTML sanitization logic
    /// </summary>
    [Parameter]
    public Func<string, string>? HtmlSanitizer { get; set; }

    /// <summary>
    /// Whether the content has code blocks
    /// </summary>
    private bool HasCodeBlocks { get; set; }

    /// <summary>
    /// Detected programming languages in code blocks
    /// </summary>
    private HashSet<string> DetectedLanguages { get; set; } = new();

    /// <summary>
    /// Processed HTML with enhanced code blocks
    /// </summary>
    private string ProcessedHtml { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ProcessHtmlContent();
    }

    /// <summary>
    /// Process the HTML content to detect and enhance code blocks
    /// </summary>
    private void ProcessHtmlContent()
    {
        if (string.IsNullOrWhiteSpace(HtmlContent))
        {
            ProcessedHtml = string.Empty;
            HasCodeBlocks = false;
            DetectedLanguages.Clear();
            return;
        }

        var content = HtmlContent;

        if (SanitizeHtml)
        {
            content = SanitizeHtmlContent(content);
        }

        // Detect code blocks and languages - more flexible pattern
        var codeBlockPattern = @"<pre[^>]*>\s*<code[^>]*>[\s\S]*?</code>\s*</pre>";
        var matches = Regex.Matches(content, codeBlockPattern, RegexOptions.IgnoreCase);

        HasCodeBlocks = matches.Count > 0;
        DetectedLanguages.Clear();

        if (!HasCodeBlocks || !EnableSyntaxHighlighting)
        {
            ProcessedHtml = content;
            return;
        }

        var processedContent = content;
        var codeBlockIndex = 0;

        foreach (Match match in matches)
        {
            var codeBlock = match.Value;

            // Then search for language within each block
            var languageMatch = Regex.Match(codeBlock, @"class\s*=\s*['""][^'""]*language-(\w+)", RegexOptions.IgnoreCase);
            var language = languageMatch.Success ? languageMatch.Groups[1].Value : "plaintext";

            // Map common language aliases
            language = MapLanguageAlias(language);

            if (!string.IsNullOrWhiteSpace(language) && language != "plaintext")
            {
                DetectedLanguages.Add(language);
            }

            // Ensure the code block has the language class
            if (!match.Groups[1].Success && language != "plaintext")
            {
                // Add language class if missing
                codeBlock = Regex.Replace(codeBlock, @"<code([^>]*)>", $"<code$1 class=\"language-{language}\">");
            }

            // Wrap code blocks if copy button is enabled
            if (EnableCopyButton)
            {
                var wrappedBlock = WrapCodeBlock(codeBlock, language, codeBlockIndex++);
                processedContent = processedContent.Replace(match.Value, wrappedBlock);
            }
            else
            {
                processedContent = processedContent.Replace(match.Value, codeBlock);
            }
        }

        // Add line-numbers class to pre elements if enabled
        if (ShowLineNumbers)
        {
            processedContent = Regex.Replace(processedContent,
                @"<pre([^>]*?)(?:class\s*=\s*['""]([^'""]*)['""])?([^>]*)>",
                m => {
                    var attrs1 = m.Groups[1].Value;
                    var existingClasses = m.Groups[2].Value;
                    var attrs2 = m.Groups[3].Value;

                    if (string.IsNullOrWhiteSpace(existingClasses))
                    {
                        return $"<pre{attrs1} class=\"line-numbers\"{attrs2}>";
                    }
                    else if (!existingClasses.Contains("line-numbers"))
                    {
                        return $"<pre{attrs1} class=\"{existingClasses} line-numbers\"{attrs2}>";
                    }
                    return m.Value;
                });
        }

        ProcessedHtml = processedContent;
    }

    /// <summary>
    /// Map common language aliases to Prism.js language names
    /// </summary>
    private string MapLanguageAlias(string language)
    {
        return language switch
        {
            "cs" => "csharp",
            "js" => "javascript",
            "ts" => "typescript",
            "py" => "python",
            "rb" => "ruby",
            "yml" => "yaml",
            "sh" => "bash",
            "ps1" => "powershell",
            "razor" => "cshtml",
            "blazor" => "cshtml",
            "html" => "markup", // HTML is included in markup
            "xml" => "markup",  // XML is also markup
            _ => language
        };
    }

    /// <summary>
    /// Check if a language component needs to be loaded
    /// </summary>
    private bool NeedsLanguageComponent(string language)
    {
        // These languages are included in Prism core
        var coreLanguages = new HashSet<string>
        {
            "markup", "html", "xml", "svg", "mathml",
            "css", "clike", "javascript", "js"
        };

        return !coreLanguages.Contains(language);
    }

    /// <summary>
    /// Sanitize HTML content to prevent XSS attacks
    /// </summary>
    private string SanitizeHtmlContent(string html)
    {
        // Use custom sanitizer if provided
        if (HtmlSanitizer is not null)
        {
            return HtmlSanitizer(html);
        }

        // Basic built-in sanitization
        // Remove script tags
        html = Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

        // Remove event handlers
        html = Regex.Replace(html, @"\bon\w+\s*=\s*['""][^'""]*['""]", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"\bon\w+\s*=\s*[^\s>]+", "", RegexOptions.IgnoreCase);

        // Remove javascript: protocol
        html = Regex.Replace(html, @"javascript\s*:", "", RegexOptions.IgnoreCase);

        // Remove iframe tags
        html = Regex.Replace(html, @"<iframe[^>]*>[\s\S]*?</iframe>", "", RegexOptions.IgnoreCase);

        // Remove object and embed tags
        html = Regex.Replace(html, @"<object[^>]*>[\s\S]*?</object>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<embed[^>]*>", "", RegexOptions.IgnoreCase);

        return html;
    }

    /// <summary>
    /// Wrap a code block with additional markup for features
    /// </summary>
    private string WrapCodeBlock(string codeBlock, string language, int index)
    {
        var languageLabel = GetLanguageLabel(language);

        return $@"
            <div class=""osirion-code-wrapper"" data-language=""{language}"">
                <div class=""osirion-code-header"">
                    <span class=""osirion-language-label"">{languageLabel}</span>
                    <button class=""{GetButtonClass()} osirion-copy-button"" aria-label=""Copy {languageLabel} code to clipboard"" onclick=""osirionCopyCode(this)"">
                        <span class=""osirion-copy-text"">Copy</span>
                    </button>
                </div>
                {codeBlock}
            </div>";
    }

    /// <summary>
    /// Get a display label for a language
    /// </summary>
    private string GetLanguageLabel(string language)
    {
        return language switch
        {
            "csharp" => "C#",
            "javascript" => "JavaScript",
            "typescript" => "TypeScript",
            "python" => "Python",
            "yaml" => "YAML",
            "bash" => "Bash",
            "powershell" => "PowerShell",
            "cshtml" => "Razor",
            "html" => "HTML",
            "css" => "CSS",
            "sql" => "SQL",
            "xml" => "XML",
            "json" => "JSON",
            _ => language.ToUpper()
        };
    }

    /// <summary>
    /// Get the CSS class for the HTML renderer
    /// </summary>
    private string GetHtmlRendererClass()
    {
        var classes = new List<string> { "osirion-html-renderer osirion-content" };

        if (UseAccessibleTheme && HasCodeBlocks)
        {
            classes.Add("osirion-wcag-theme");
        }

        if (!string.IsNullOrWhiteSpace(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Generate the initialization script for syntax highlighting features
    /// </summary>
    private string GetInitializationScript()
    {
        return $@"
            (function() {{
                // Simple initialization with retry
                var attempts = 0;
                var maxAttempts = 50;
    
                function initializeHighlighting() {{
                    attempts++;
        
                    if (typeof Prism === 'undefined') {{
                        if (attempts < maxAttempts) {{
                            setTimeout(initializeHighlighting, 100);
                        }}
                        return;
                    }}

                    // Highlight all code blocks
                    Prism.highlightAll();

                    {(EnableCopyButton ? GetCopyButtonScript() : "")}
                    {(EnableLineHighlighting && ShowLineNumbers ? GetLineHighlightingScript() : "")}
                }}

                // Start initialization
                setTimeout(initializeHighlighting, 100);
            }})();
            ";
    }

    /// <summary>
    /// Generate the copy button functionality script
    /// </summary>
    private string GetCopyButtonScript()
    {
        return @"
        // Copy button functionality
        document.querySelectorAll('.osirion-copy-button').forEach(button => {
            button.addEventListener('click', async () => {
                const codeBlock = button.closest('.osirion-code-wrapper').querySelector('code');
                const text = codeBlock.textContent;
                
                try {
                    await navigator.clipboard.writeText(text);
                    button.textContent = 'Copied!';
                    button.classList.add('copied');
                    
                    // Announce to screen readers
                    const announcement = document.createElement('div');
                    announcement.className = 'sr-only';
                    announcement.setAttribute('aria-live', 'polite');
                    announcement.textContent = 'Code copied to clipboard';
                    document.body.appendChild(announcement);
                    
                    setTimeout(() => {
                        button.textContent = 'Copy';
                        button.classList.remove('copied');
                        announcement.remove();
                    }, 2000);
                } catch (err) {
                    console.error('Failed to copy:', err);
                    button.textContent = 'Error';
                    setTimeout(() => {
                        button.textContent = 'Copy';
                    }, 2000);
                }
            });
        });";
    }

    /// <summary>
    /// Generate the line highlighting functionality script
    /// </summary>
    private string GetLineHighlightingScript()
    {
        return @"
        // Line highlighting functionality
        const highlightedLines = new Map();
        
        // Function to create highlight overlays
        function createHighlightOverlays(pre) {
            pre.querySelectorAll('.osirion-line-highlight-overlay').forEach(el => el.remove());
            
            const codeId = Array.from(document.querySelectorAll('pre')).indexOf(pre);
            const lines = highlightedLines.get(codeId) || [];
            const lineHeight = parseFloat(window.getComputedStyle(pre).lineHeight);
            const codeElement = pre.querySelector('code');
            const paddingTop = parseFloat(window.getComputedStyle(codeElement).paddingTop);
            
            lines.forEach(lineNum => {
                const overlay = document.createElement('div');
                overlay.className = 'osirion-line-highlight-overlay';
                overlay.style.top = `${paddingTop + (lineNum - 1) * lineHeight}px`;
                pre.appendChild(overlay);
            });
        }
        
        // Make line numbers clickable
        document.querySelectorAll('.line-numbers pre').forEach((pre) => {
            const lineElements = pre.querySelectorAll('.line-numbers-rows > span');
            
            lineElements.forEach((span, index) => {
                const lineNum = index + 1;
                
                span.setAttribute('role', 'button');
                span.setAttribute('aria-label', `Toggle highlight for line ${lineNum}`);
                span.setAttribute('tabindex', '0');
                
                function toggleLine(e) {
                    const codeId = Array.from(document.querySelectorAll('pre')).indexOf(pre);
                    let currentHighlights = highlightedLines.get(codeId) || [];
                    const isCtrlPressed = e && (e.ctrlKey || e.metaKey);
                    
                    if (span.classList.contains('osirion-line-highlighted')) {
                        span.classList.remove('osirion-line-highlighted');
                        currentHighlights = currentHighlights.filter(n => n !== lineNum);
                    } else {
                        if (!isCtrlPressed) {
                            pre.querySelectorAll('.osirion-line-highlighted').forEach(el => {
                                el.classList.remove('osirion-line-highlighted');
                            });
                            currentHighlights = [];
                        }
                        
                        span.classList.add('osirion-line-highlighted');
                        if (!currentHighlights.includes(lineNum)) {
                            currentHighlights.push(lineNum);
                        }
                    }
                    
                    currentHighlights.sort((a, b) => a - b);
                    highlightedLines.set(codeId, currentHighlights);
                    createHighlightOverlays(pre);
                }
                
                span.addEventListener('click', toggleLine);
                span.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        toggleLine(e);
                    }
                });
            });
        });";
    }

    /// <summary>
    /// Get the accessible theme CSS
    /// </summary>
    private string GetAccessibleThemeCss()
    {
        return @"
            /* WCAG AAA Compliant Prism Theme */
            code[class*=""language-""],
            pre[class*=""language-""] {
                color: #f8f8f2;
                background: none;
                text-shadow: 0 1px rgba(0, 0, 0, 0.3);
                font-family: Consolas, Monaco, 'Andale Mono', 'Ubuntu Mono', monospace;
                font-size: 1em;
                text-align: left;
                white-space: pre;
                word-spacing: normal;
                word-break: normal;
                word-wrap: normal;
                line-height: 1.5;
                tab-size: 4;
                hyphens: none;
            }

            pre[class*=""language-""] {
                padding: 1em;
                margin: 0.5em 0;
                overflow: auto;
                border-radius: 0.3em;
                background: #1e1e1e;
            }

            :not(pre) > code[class*=""language-""] {
                padding: 0.1em 0.3em;
                border-radius: 0.3em;
                white-space: normal;
                background: #1e1e1e;
            }

            .token.comment,
            .token.prolog,
            .token.doctype,
            .token.cdata {
                color: #8b949e;
            }

            .token.punctuation {
                color: #f8f8f2;
            }

            .token.namespace {
                opacity: 0.9;
            }

            .token.property,
            .token.tag,
            .token.constant,
            .token.symbol,
            .token.deleted {
                color: #ff79c6;
            }

            .token.boolean,
            .token.number {
                color: #bd93f9;
            }

            .token.selector,
            .token.attr-name,
            .token.string,
            .token.char,
            .token.builtin,
            .token.inserted {
                color: #50fa7b;
            }

            .token.operator,
            .token.entity,
            .token.url,
            .language-css .token.string,
            .style .token.string,
            .token.variable {
                color: #f8f8f2;
            }

            .token.atrule,
            .token.attr-value,
            .token.function,
            .token.class-name {
                color: #f1fa8c;
            }

            .token.keyword {
                color: #8be9fd;
            }

            .token.regex,
            .token.important {
                color: #ffb86c;
            }

            .token.important,
            .token.bold {
                font-weight: bold;
            }

            .token.italic {
                font-style: italic;
            }

            .token.entity {
                cursor: help;
            }";
    }
}