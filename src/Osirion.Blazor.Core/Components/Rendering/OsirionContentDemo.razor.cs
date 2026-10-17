using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Components;

/// <summary>
/// Renders a registered content demo: the live component, and a Code tab with its source when the demo has one.
/// <see cref="OsirionHtmlRenderer"/> renders it for fenced <c>demo</c> blocks in content; pages can also use it
/// directly with <see cref="Key"/>.
/// </summary>
public partial class OsirionContentDemo
{
    [Inject]
    private IServiceProvider Services { get; set; } = default!;

    /// <summary>
    /// Gets or sets the demo to render, with its component, parameters, title and source. Takes precedence over
    /// <see cref="Key"/>.
    /// </summary>
    [Parameter]
    public ContentDemo? Demo { get; set; }

    /// <summary>
    /// Gets or sets the key of a demo registered with <c>AddOsirionContentDemos</c>, used when <see cref="Demo"/>
    /// is not set. The component renders nothing when neither resolves to a demo.
    /// </summary>
    [Parameter]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets whether the source code on the Code tab has a language header and a Copy button. The button
    /// needs the copy script that <see cref="OsirionHtmlRenderer"/> loads.
    /// </summary>
    [Parameter]
    public bool EnableCopyButton { get; set; } = true;

    /// <summary>Gets or sets the label of the preview tab. Shown only when the demo has source code.</summary>
    [Parameter]
    public string PreviewText { get; set; } = "Preview";

    /// <summary>Gets or sets the label of the Code tab. Shown only when the demo has source code.</summary>
    [Parameter]
    public string CodeText { get; set; } = "Code";

    private ContentDemo? ResolvedDemo { get; set; }

    private IDictionary<string, object>? ComponentParameters { get; set; }

    private MarkupString? CodeMarkup { get; set; }

    private string ExampleLabel => $"{ResolvedDemo?.Title ?? ResolvedDemo?.Key} example";

    // Each instance gets its own tab group name, so several demos on a page do not share radio buttons.
    private string TabsId { get; } = "osirion-demo-" + Guid.NewGuid().ToString("N")[..8];

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        ResolvedDemo = Demo;
        if (ResolvedDemo is null && !string.IsNullOrWhiteSpace(Key)
            && Services.GetService<IContentDemoRegistry>() is { } registry
            && registry.TryGet(Key, out var registered))
        {
            ResolvedDemo = registered;
        }

        ComponentParameters = ResolvedDemo?.Parameters?
            .Where(parameter => parameter.Value is not null)
            .ToDictionary(parameter => parameter.Key, parameter => parameter.Value!);

        CodeMarkup = ResolvedDemo?.Source is { } source ? new MarkupString(BuildCodeBlock(source)) : null;
    }

    // The source is encoded before it becomes markup, so a demo's code is always shown, never run.
    private string BuildCodeBlock(string source)
    {
        var block = $"<pre><code class=\"language-razor\">{WebUtility.HtmlEncode(source.Trim())}</code></pre>";
        return EnableCopyButton ? CodeBlockMarkup.Wrap(block, "cshtml", "Razor", GetButtonClass()) : block;
    }

    private string GetDemoClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-content-demo" : $"osirion-content-demo {Class}";
}
