using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Extensions;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Sanitizes the content HTML a component writes as raw markup, and keeps the result until the content changes.
/// </summary>
/// <remarks>
/// Built-in providers sanitize at ingestion; this also covers custom providers and items set by the caller.
/// A component renders again on every state change, such as a table-of-contents click or a parent render, and
/// sanitizing parses the whole document, so the result is reused while the content is the same.
/// One instance belongs to one component instance and is used from its render only.
/// </remarks>
internal sealed class SanitizedContentCache
{
    private string? _content;
    private MarkupString? _sanitizedContent;

    /// <summary>
    /// Returns <paramref name="content"/> sanitized by the registered <c>IHtmlContentSanitizer</c>, or by the default
    /// allow-list when none is registered.
    /// </summary>
    /// <param name="serviceProvider">The component's service provider.</param>
    /// <param name="content">The content HTML. May be <see langword="null"/>.</param>
    /// <returns>The sanitized markup.</returns>
    public MarkupString Get(IServiceProvider serviceProvider, string? content)
    {
        if (_sanitizedContent is null || !string.Equals(_content, content, StringComparison.Ordinal))
        {
            _sanitizedContent = new MarkupString(serviceProvider.GetHtmlContentSanitizer().Sanitize(content));
            _content = content;
        }

        return _sanitizedContent.Value;
    }
}
