using System.Collections.Frozen;
using Ganss.Xss;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;

namespace Osirion.Blazor.Core.Services.Implementations;

/// <summary>
/// Allow-list HTML sanitizer for content, built on the HtmlSanitizer library (AngleSharp HTML5 parser).
/// </summary>
/// <remarks>
/// <para>
/// The input is parsed the way a browser parses it and only allowed elements, attributes and URL schemes
/// are written back, so encoded or malformed markup cannot slip through the way it can with pattern
/// matching. See <see cref="HtmlContentSanitizerOptions"/> for the allow-list.
/// </para>
/// <para>
/// A disallowed element is unwrapped: the element goes and its sanitized content stays, so an unknown tag
/// in prose does not take the text after it with it. Elements whose content is code, an embedded document
/// or foreign markup are removed together with their content.
/// </para>
/// <para>
/// The allow-list is copied when the instance is created and never changes afterwards, so one instance can
/// be shared by concurrent callers.
/// </para>
/// </remarks>
public sealed class HtmlContentSanitizer : IHtmlContentSanitizer
{
    /// <summary>
    /// Elements removed together with their content rather than unwrapped, because their content is script,
    /// CSS, an embedded document or foreign markup rather than text meant for the reader.
    /// </summary>
    private static readonly FrozenSet<string> ContentRemovingTags = new[]
    {
        "script", "style", "template", "noscript", "iframe", "frame", "frameset",
        "object", "embed", "applet", "svg", "math"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The library marks <c>data</c> as a URL attribute because <c>&lt;object data&gt;</c> loads a resource.
    /// Object is never allowed here, so the name is only ever a plain attribute, for example on a custom
    /// element that carries chart data, and URL screening would corrupt values that contain a colon.
    /// </summary>
    private const string ObjectDataAttribute = "data";

    private readonly Ganss.Xss.HtmlSanitizer _sanitizer;
    private readonly FrozenDictionary<string, FrozenSet<string>> _elementAttributes;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlContentSanitizer"/> class.
    /// </summary>
    /// <param name="options">The allow-list configuration.</param>
    public HtmlContentSanitizer(IOptions<HtmlContentSanitizerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var settings = options.Value;

        _elementAttributes = settings.ElementAttributes
            .Where(entry => !HtmlContentSanitizerOptions.NeverAllowedTags.Contains(entry.Key))
            .ToFrozenDictionary(
                entry => entry.Key,
                entry => entry.Value.Where(name => !HtmlContentSanitizerOptions.IsEventHandlerAttribute(name))
                    .ToFrozenSet(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

        _sanitizer = new Ganss.Xss.HtmlSanitizer(new HtmlSanitizerOptions
        {
            AllowedTags = settings.AllowedTags
                .Where(tag => !HtmlContentSanitizerOptions.NeverAllowedTags.Contains(tag))
                .ToHashSet(StringComparer.OrdinalIgnoreCase),
            AllowedAttributes = settings.AllowedAttributes
                .Where(name => !HtmlContentSanitizerOptions.IsEventHandlerAttribute(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase),
            AllowedSchemes = settings.AllowedSchemes
                .Where(scheme => !HtmlContentSanitizerOptions.NeverAllowedSchemes.Contains(scheme))
                .ToHashSet(StringComparer.OrdinalIgnoreCase),
            UriAttributes = HtmlSanitizerDefaults.UriAttributes
                .Where(name => !string.Equals(name, ObjectDataAttribute, StringComparison.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.OrdinalIgnoreCase),
            UriListAttributes = new HashSet<string>(HtmlSanitizerDefaults.UriListAttributes, StringComparer.OrdinalIgnoreCase),
            AllowedCssProperties = new HashSet<string>(HtmlSanitizerDefaults.AllowedCssProperties, StringComparer.OrdinalIgnoreCase),
            AllowedAtRules = new HashSet<AngleSharp.Css.Dom.CssRuleType>(HtmlSanitizerDefaults.AllowedAtRules),
        })
        {
            KeepChildNodes = true
        };

        _sanitizer.RemovingTag += OnRemovingTag;
        _sanitizer.RemovingAttribute += OnRemovingAttribute;
        _sanitizer.FilterUrl += OnFilterUrl;
    }

    /// <summary>
    /// Gets a sanitizer with the default allow-list, for callers that have no service provider or whose
    /// host did not register <see cref="IHtmlContentSanitizer"/>.
    /// </summary>
    public static IHtmlContentSanitizer Default { get; } =
        new HtmlContentSanitizer(Options.Create(new HtmlContentSanitizerOptions()));

    /// <inheritdoc/>
    public string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        return _sanitizer.Sanitize(html);
    }

    private static void OnRemovingTag(object? sender, RemovingTagEventArgs e)
    {
        if (!ContentRemovingTags.Contains(e.Tag.LocalName))
        {
            return;
        }

        // Remove the element with its content here; cancelling stops the library from unwrapping it.
        e.Tag.Remove();
        e.Cancel = true;
    }

    private void OnRemovingAttribute(object? sender, RemovingAttributeEventArgs e)
    {
        // Only an attribute that is merely not on the global allow-list is rescued. A value removed for
        // an unsafe URL or script include stays removed even on an element that allows the attribute.
        if (e.Reason == RemoveReason.NotAllowedAttribute
            && _elementAttributes.TryGetValue(e.Tag.LocalName, out var allowed)
            && allowed.Contains(e.Attribute.Name)
            && !HtmlContentSanitizerOptions.IsEventHandlerAttribute(e.Attribute.Name))
        {
            e.Cancel = true;
        }
    }

    private static void OnFilterUrl(object? sender, FilterUrlEventArgs e)
    {
        // The library reads everything before the first colon as a scheme, so it drops relative URLs such as
        // "page.html?t=10:30". A browser resolves a URL without a scheme against the page address, where it
        // cannot run script, so such a URL is kept as written.
        if (e.SanitizedUrl is null && !HasUrlScheme(e.OriginalUrl))
        {
            e.SanitizedUrl = e.OriginalUrl;
        }
    }

    /// <summary>
    /// Determines whether a browser reads <paramref name="url"/> as having a scheme: an ASCII letter, then
    /// ASCII letters, digits, <c>+</c>, <c>-</c> or <c>.</c>, then a colon (WHATWG URL Standard).
    /// </summary>
    /// <remarks>
    /// Browsers drop control characters and spaces at either end, and tabs and newlines anywhere, before they
    /// look for a scheme. Every control and white-space character is skipped here, which can only make more
    /// URLs look like they have a scheme, so any doubt ends in the URL being removed.
    /// </remarks>
    private static bool HasUrlScheme(string url)
    {
        var schemeLength = 0;

        foreach (var character in url)
        {
            if (char.IsControl(character) || char.IsWhiteSpace(character))
            {
                continue;
            }

            if (character == ':')
            {
                return schemeLength > 0;
            }

            var isSchemeCharacter = schemeLength == 0
                ? char.IsAsciiLetter(character)
                : char.IsAsciiLetterOrDigit(character) || character is '+' or '-' or '.';

            if (!isSchemeCharacter)
            {
                return false;
            }

            schemeLength++;
        }

        return false;
    }
}
