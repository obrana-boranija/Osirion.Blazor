using System.Collections.Frozen;
using Ganss.Xss;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Core.Configuration;

/// <summary>
/// Configures the allow-list that <see cref="IHtmlContentSanitizer"/> applies to content HTML.
/// </summary>
/// <remarks>
/// <para>
/// The defaults start from the allow-list of the HtmlSanitizer library, add <c>class</c> and <c>id</c>
/// (needed for content styling, code highlighting and table of contents anchors) and remove forms,
/// form controls and the <c>style</c> and <c>name</c> attributes. Iframes and <c>&lt;style&gt;</c> elements
/// are not allowed by default either. A host can opt in to any of them through these options.
/// </para>
/// <para>
/// Some markup is never allowed, whatever these options contain: the elements <c>script</c>, <c>svg</c>,
/// <c>math</c>, <c>template</c>, <c>object</c>, <c>embed</c>, <c>applet</c>, <c>meta</c>, <c>base</c>,
/// <c>link</c>, <c>frame</c>, <c>frameset</c> and <c>noscript</c>, event handler attributes (any name that
/// starts with <c>on</c>), and the URL schemes <c>javascript</c>, <c>vbscript</c> and <c>data</c>.
/// </para>
/// </remarks>
/// <example>
/// Allow a custom <c>&lt;chart&gt;</c> element whose attributes are read by client-side script:
/// <code>
/// builder.Services.AddOsirionHtmlSanitizer(options =&gt;
///     options.AllowElement("chart", "type", "title", "caption", "data", "colors"));
/// </code>
/// The sanitizer only makes those attribute values inert as HTML. The script that reads them must treat them
/// as text (<c>textContent</c>, <c>setAttribute</c>, <c>createElement</c>) and never write them through
/// <c>innerHTML</c>, because the browser decodes a value such as <c>&amp;lt;img onerror=...&amp;gt;</c> back
/// into markup.
/// </example>
public sealed class HtmlContentSanitizerOptions
{
    private static readonly FrozenSet<string> FormTags = new[]
    {
        "form", "input", "button", "select", "textarea", "option", "optgroup", "fieldset",
        "label", "legend", "datalist", "keygen", "output", "menuitem"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> FormAttributes = new[]
    {
        "action", "method", "enctype"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Attributes on the library's default list that are kept only when a host opts in: <c>style</c>, and
    /// <c>name</c>, which on an image adds a named property to <c>document</c> that hides built-in members
    /// such as <c>document.querySelectorAll</c> and breaks the page's scripts.
    /// </summary>
    private static readonly FrozenSet<string> OptInAttributes = new[]
    {
        "style", "name"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, HashSet<string>> _elementAttributes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Elements that are removed even when a host adds them to <see cref="AllowedTags"/>, because they run
    /// script, load active content, change how the page resolves URLs, or switch the parser into a foreign
    /// namespace (the cause of known sanitizer bypasses for <c>svg</c>, <c>math</c> and <c>template</c>).
    /// </summary>
    internal static readonly FrozenSet<string> NeverAllowedTags = new[]
    {
        "script", "svg", "math", "template", "object", "embed", "applet",
        "meta", "base", "link", "frame", "frameset", "noscript"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// URL schemes that are removed even when a host adds them to <see cref="AllowedSchemes"/>.
    /// </summary>
    internal static readonly FrozenSet<string> NeverAllowedSchemes = new[]
    {
        "javascript", "vbscript", "data"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the HTML element names that are kept. Names are case-insensitive.
    /// </summary>
    /// <remarks>
    /// Add <c>style</c> to allow <c>&lt;style&gt;</c> elements (their CSS is still filtered) or <c>iframe</c>
    /// to allow iframes. Prefer <see cref="AllowElement"/> for iframes, so that only the attributes an embed
    /// needs are kept on them. Names in the never-allowed list are ignored.
    /// </remarks>
    public ISet<string> AllowedTags { get; } = new HashSet<string>(
        HtmlSanitizerDefaults.AllowedTags.Where(tag => !FormTags.Contains(tag)),
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the attribute names that are kept on every allowed element. Names are case-insensitive.
    /// </summary>
    /// <remarks>
    /// Add <c>style</c> to keep inline styles (their CSS properties are still filtered). Add <c>name</c> only
    /// when content needs it, for example for image maps: on images it can hide built-in members of
    /// <c>document</c>. Event handler attributes are ignored. URL attributes such as <c>href</c> and
    /// <c>src</c> are always checked against <see cref="AllowedSchemes"/>.
    /// </remarks>
    public ISet<string> AllowedAttributes { get; } = new HashSet<string>(
        HtmlSanitizerDefaults.AllowedAttributes
            .Where(attribute => !FormAttributes.Contains(attribute) && !OptInAttributes.Contains(attribute))
            .Concat(["class", "id"]),
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the URL schemes allowed in URL attributes such as <c>href</c> and <c>src</c>.
    /// Relative URLs are always allowed. Defaults to <c>http</c>, <c>https</c>, <c>mailto</c> and <c>tel</c>.
    /// </summary>
    public ISet<string> AllowedSchemes { get; } = new HashSet<string>(
        ["http", "https", "mailto", "tel"],
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the attributes allowed only on a specific element, keyed by element name.
    /// </summary>
    internal IReadOnlyDictionary<string, HashSet<string>> ElementAttributes => _elementAttributes;

    /// <summary>
    /// Allows an element, typically a custom element read by client-side script, together with attributes
    /// that are kept only on that element.
    /// </summary>
    /// <param name="tagName">The element name, for example <c>chart</c>.</param>
    /// <param name="attributeNames">
    /// Attribute names to keep on <paramref name="tagName"/> in addition to <see cref="AllowedAttributes"/>.
    /// Values of URL attributes such as <c>src</c> are still checked against <see cref="AllowedSchemes"/>.
    /// </param>
    /// <remarks>
    /// Other attribute values are kept as written, so they are safe only while they stay attribute text.
    /// Client-side script that reads them must not write them through <c>innerHTML</c> or similar sinks.
    /// </remarks>
    /// <returns>The same options instance, for chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="tagName"/> is empty or never allowed, or when an attribute name is empty
    /// or is an event handler attribute.
    /// </exception>
    public HtmlContentSanitizerOptions AllowElement(string tagName, params string[] attributeNames)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);
        ArgumentNullException.ThrowIfNull(attributeNames);

        if (NeverAllowedTags.Contains(tagName))
        {
            throw new ArgumentException($"The element '{tagName}' can never be allowed in content.", nameof(tagName));
        }

        foreach (var attributeName in attributeNames)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentException("Attribute names cannot be empty.", nameof(attributeNames));
            }

            if (IsEventHandlerAttribute(attributeName))
            {
                throw new ArgumentException($"The event handler attribute '{attributeName}' can never be allowed in content.", nameof(attributeNames));
            }
        }

        AllowedTags.Add(tagName);

        if (!_elementAttributes.TryGetValue(tagName, out var allowed))
        {
            allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _elementAttributes[tagName] = allowed;
        }

        allowed.UnionWith(attributeNames);

        return this;
    }

    /// <summary>
    /// Determines whether an attribute name is an inline event handler such as <c>onclick</c>.
    /// </summary>
    internal static bool IsEventHandlerAttribute(string attributeName) =>
        attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase);
}
