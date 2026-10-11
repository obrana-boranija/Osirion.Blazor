using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;
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
    /// <remarks>
    /// This parameter has never been read by the component. Style code blocks with CSS selectors on the
    /// <c>language-*</c> classes instead.
    /// </remarks>
    [Obsolete("LanguageClasses is not used by the component. Style code blocks with CSS selectors on the language-* classes instead.")]
    [Parameter]
    public Dictionary<string, string>? LanguageClasses { get; set; }

    /// <summary>
    /// Whether to sanitize the HTML content before rendering it. Defaults to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Sanitization uses the registered <see cref="IHtmlContentSanitizer"/> (see <c>AddOsirionHtmlSanitizer</c>),
    /// or the default allow-list when none is registered. Set to <see langword="false"/> only for markup the
    /// application itself produced; never for content, user input or anything read from a repository.
    /// </remarks>
    [Parameter]
    public bool SanitizeHtml { get; set; } = true;

    /// <summary>
    /// Optional custom sanitizer used instead of the registered <see cref="IHtmlContentSanitizer"/>
    /// when <see cref="SanitizeHtml"/> is <see langword="true"/>.
    /// </summary>
    [Parameter]
    public Func<string, string>? HtmlSanitizer { get; set; }

    /// <summary>
    /// Optional base address Prism is loaded from, for hosts that serve it themselves (for example
    /// <c>/lib/prism/1.29.0/</c>). When empty, the pinned cdnjs release is used and every file is verified with its
    /// Subresource Integrity hash.
    /// </summary>
    /// <remarks>
    /// Files from a custom base address are loaded without an integrity check, because the library cannot know
    /// their content. Expected layout: <c>prism.min.js</c>, <c>components/prism-{language}.min.js</c>,
    /// <c>themes/prism-okaidia.min.css</c> and <c>plugins/line-numbers/...</c>, as in the Prism release.
    /// </remarks>
    [Parameter]
    public string? PrismBaseUrl { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

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

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ProcessHtmlContent();
    }

    /// <summary>
    /// Matches a code block: a pre element whose first child is a code element.
    /// </summary>
    private static readonly Regex CodeBlockPattern = new(
        @"<pre[^>]*>\s*<code[^>]*>[\s\S]*?</code>\s*</pre>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex LanguageClassPattern = new(
        @"class\s*=\s*['""][^'""]*language-(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Matches the opening pre tag at the start of a code block, attribute by attribute, so that markup held in a
    /// quoted attribute value is never mistaken for the end of the tag.
    /// </summary>
    private static readonly Regex OpeningPreTagPattern = new(
        @"^<pre((?:\s+[^\s""'<>/=]+(?:\s*=\s*(?:""[^""]*""|'[^']*'|[^\s""'=<>`]+))?)*)\s*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex AttributePattern = new(
        @"(\s+)([^\s""'<>/=]+)(?:(\s*=\s*)(""[^""]*""|'[^']*'|[^\s""'=<>`]+))?", RegexOptions.Compiled);

    private const string LineNumbersClass = "line-numbers";

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

        // Sanitize before the code blocks are wrapped: the wrapper adds the library's own copy button handler.
        if (SanitizeHtml)
        {
            content = SanitizeHtmlContent(content);
        }

        HasCodeBlocks = CodeBlockPattern.IsMatch(content);
        DetectedLanguages.Clear();

        if (!HasCodeBlocks || !EnableSyntaxHighlighting)
        {
            ProcessedHtml = content;
            return;
        }

        // One pass over the content: every block is rewritten where it stands, so identical blocks are each
        // handled once and a wrapped block is never searched again.
        var codeBlockIndex = 0;
        ProcessedHtml = CodeBlockPattern.Replace(content, match =>
        {
            var codeBlock = match.Value;

            var languageMatch = LanguageClassPattern.Match(codeBlock);
            var language = MapLanguageAlias(languageMatch.Success ? languageMatch.Groups[1].Value : "plaintext");

            if (!string.IsNullOrWhiteSpace(language) && language != "plaintext")
            {
                DetectedLanguages.Add(language);
            }

            // The block keeps its own language class, which Prism reads from the code element or its pre
            // ancestor. Only the class list of the opening pre tag is touched, and only through a tag-aware
            // match: this runs after sanitization, and a pattern that stops at the first '>' can turn markup
            // held in a quoted attribute value into a live element.
            if (ShowLineNumbers)
            {
                codeBlock = AddLineNumbersClass(codeBlock);
            }

            return EnableCopyButton ? WrapCodeBlock(codeBlock, language, codeBlockIndex++) : codeBlock;
        });
    }

    /// <summary>
    /// Adds the Prism line-numbers class to the opening pre tag of a code block, keeping the classes it has.
    /// </summary>
    private static string AddLineNumbersClass(string codeBlock)
    {
        var tag = OpeningPreTagPattern.Match(codeBlock);
        if (!tag.Success)
        {
            return codeBlock;
        }

        var hasClassAttribute = false;
        var attributes = AttributePattern.Replace(tag.Groups[1].Value, attribute =>
        {
            if (hasClassAttribute || !attribute.Groups[2].Value.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                return attribute.Value;
            }

            hasClassAttribute = true;
            var rawValue = attribute.Groups[4].Value;
            var quote = rawValue.Length > 0 && rawValue[0] is '"' or '\'' ? rawValue[0] : '"';
            var classes = rawValue.Trim('"', '\'');

            if (classes.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Contains(LineNumbersClass, StringComparer.Ordinal))
            {
                return attribute.Value;
            }

            var combined = string.IsNullOrWhiteSpace(classes) ? LineNumbersClass : $"{classes} {LineNumbersClass}";
            return $"{attribute.Groups[1].Value}class{attribute.Groups[3].Value}{quote}{combined}{quote}";
        });

        if (!hasClassAttribute)
        {
            attributes += $" class=\"{LineNumbersClass}\"";
        }

        return string.Concat("<pre", attributes, ">", codeBlock.AsSpan(tag.Length));
    }

    /// <summary>
    /// Common language aliases and the Prism.js language names they stand for. HTML and XML are both Prism's markup.
    /// </summary>
    private static readonly Dictionary<string, string> PrismLanguageAliases = new(StringComparer.Ordinal)
    {
        ["cs"] = "csharp",
        ["js"] = "javascript",
        ["ts"] = "typescript",
        ["py"] = "python",
        ["rb"] = "ruby",
        ["yml"] = "yaml",
        ["sh"] = "bash",
        ["ps1"] = "powershell",
        ["razor"] = "cshtml",
        ["blazor"] = "cshtml",
        ["html"] = "markup",
        ["xml"] = "markup",
    };

    /// <summary>
    /// Display labels for Prism language names; a language without one is shown in upper case.
    /// </summary>
    private static readonly Dictionary<string, string> LanguageLabels = new(StringComparer.Ordinal)
    {
        ["csharp"] = "C#",
        ["javascript"] = "JavaScript",
        ["typescript"] = "TypeScript",
        ["python"] = "Python",
        ["yaml"] = "YAML",
        ["bash"] = "Bash",
        ["powershell"] = "PowerShell",
        ["cshtml"] = "Razor",
        ["html"] = "HTML",
        ["css"] = "CSS",
        ["sql"] = "SQL",
        ["xml"] = "XML",
        ["json"] = "JSON",
    };

    /// <summary>
    /// Map common language aliases to Prism.js language names
    /// </summary>
    private static string MapLanguageAlias(string language) =>
        PrismLanguageAliases.TryGetValue(language, out var prismLanguage) ? prismLanguage : language;

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
        if (HtmlSanitizer is not null)
        {
            return HtmlSanitizer(html);
        }

        return ServiceProvider.GetHtmlContentSanitizer().Sanitize(html);
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
    private static string GetLanguageLabel(string language) =>
        LanguageLabels.TryGetValue(language, out var label) ? label : language.ToUpper();

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

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the address of a Prism file: the pinned cdnjs release, or <see cref="PrismBaseUrl"/> when it is set.
    /// </summary>
    private string PrismUrl(string relativePath) => string.IsNullOrWhiteSpace(PrismBaseUrl)
        ? PrismCdnAssets.BaseUrl + relativePath
        : PrismBaseUrl.TrimEnd('/') + "/" + relativePath;

    /// <summary>
    /// Gets the Subresource Integrity hash of a pinned Prism file, or <see langword="null"/> when the file comes
    /// from a custom <see cref="PrismBaseUrl"/> (the host owns that source) or is not part of the pinned release.
    /// </summary>
    private string? PrismIntegrity(string relativePath) =>
        string.IsNullOrWhiteSpace(PrismBaseUrl) && PrismCdnAssets.TryGetIntegrity(relativePath, out var integrity)
            ? integrity
            : null;

    private string? PrismCrossOrigin(string relativePath) => PrismIntegrity(relativePath) is null ? null : "anonymous";

    /// <summary>
    /// Lists the Prism scripts to load, in execution order: the core first, then the language components found in
    /// the content, then the line-numbers plugin.
    /// </summary>
    private List<Dictionary<string, object?>> GetPrismScripts()
    {
        var usesPinnedRelease = string.IsNullOrWhiteSpace(PrismBaseUrl);
        var relativePaths = new List<string> { "prism.min.js" };

        // Without a custom source, a language that is not part of the pinned release is skipped instead of
        // requested: it has no integrity hash and the request could only fail.
        relativePaths.AddRange(DetectedLanguages
            .Where(NeedsLanguageComponent)
            .Order(StringComparer.Ordinal)
            .Select(language => $"components/prism-{language}.min.js")
            .Where(path => !usesPinnedRelease || PrismCdnAssets.TryGetIntegrity(path, out _)));

        if (ShowLineNumbers)
        {
            relativePaths.Add("plugins/line-numbers/prism-line-numbers.min.js");
        }

        return relativePaths
            .Select(path => new Dictionary<string, object?>
            {
                ["src"] = PrismUrl(path),
                ["integrity"] = PrismIntegrity(path),
                ["required"] = path == "prism.min.js"
            })
            .ToList();
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
