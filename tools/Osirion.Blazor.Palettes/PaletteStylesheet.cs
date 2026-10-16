using System.Text;

namespace Osirion.Blazor.Palettes;

/// <summary>Writes the palette variants and the code highlighting rules as <c>palettes.css</c> and <c>palettes.min.css</c>.</summary>
internal static class PaletteStylesheet
{
    private const string Scope = ":root[data-osirion-palette][data-osirion-theme]";

    // Prism's token classes and the code token that colours them. Prism's own themes group them the same way.
    private static readonly (string Token, string[] Classes)[] PrismRules =
    [
        ("code-comment", ["comment", "prolog", "doctype", "cdata"]),
        ("code-text", ["punctuation", "operator", "entity", "url"]),
        ("code-keyword", ["keyword", "atrule", "important"]),
        ("code-string", ["string", "char", "attr-value", "template-string"]),
        ("code-number", ["number", "boolean", "constant", "symbol"]),
        ("code-function", ["function", "function-name"]),
        ("code-type", ["class-name", "builtin", "namespace"]),
        ("code-variable", ["variable", "property", "parameter"]),
        ("code-tag", ["tag", "selector"]),
        ("code-attribute", ["attr-name"]),
        ("code-regex", ["regex"]),
        ("code-inserted", ["inserted"]),
        ("code-deleted", ["deleted"]),
    ];

    /// <summary>Builds every variant; the colours moved to reach a contrast target are reported to <paramref name="adjustments"/>.</summary>
    public static IReadOnlyList<PaletteVariant> BuildVariants(ICollection<string> adjustments) =>
    [
        .. PaletteDefinitions.OsirionCode.Select(code => PaletteBuilder.BuildCodeOnly("osirion", code.Mode, code.Code, adjustments)),
        .. PaletteDefinitions.All.Select(spec => PaletteBuilder.Build(spec, adjustments)),
    ];

    /// <summary>Generates the readable and the minified stylesheet.</summary>
    public static (string Css, string MinifiedCss) Generate(IReadOnlyList<PaletteVariant> variants)
    {
        var css = new StringBuilder();
        css.Append("/*\n");
        // The raw string takes the source file's line endings, which depend on the checkout.
        foreach (var line in PaletteDefinitions.Header.ReplaceLineEndings("\n").Split('\n'))
        {
            css.Append(line.Length == 0 ? " *\n" : $" * {line.TrimEnd()}\n");
        }

        css.Append(" */\n");

        var minified = new StringBuilder();
        minified.Append("/*! Osirion palettes. Colour sources and licenses: THIRD-PARTY-NOTICES.md in Osirion.Blazor.Theming */");

        foreach (var variant in variants)
        {
            css.Append('\n').Append(string.Join(",\n", variant.Selectors)).Append(" {\n");
            foreach (var (name, value) in variant.Tokens)
            {
                css.Append($"    --osirion-{name}: {value};\n");
            }

            css.Append("}\n");
            minified.Append(string.Join(',', variant.Selectors)).Append('{')
                .Append(string.Join(';', variant.Tokens.Select(token => $"--osirion-{token.Key}:{token.Value}")))
                .Append('}');
        }

        css.Append("\n/* Code highlighting follows the palette: these rules outrank Prism's themes, including the accessible one */\n");
        foreach (var (selectors, declarations) in CodeRules())
        {
            css.Append('\n').Append(string.Join(",\n", selectors)).Append(" {\n");
            foreach (var declaration in declarations)
            {
                css.Append($"    {declaration};\n");
            }

            css.Append("}\n");
            minified.Append(string.Join(',', selectors)).Append('{')
                .Append(string.Join(';', declarations.Select(declaration => declaration.Replace(": ", ":", StringComparison.Ordinal))))
                .Append('}');
        }

        return (css.ToString(), minified.Append('\n').ToString());
    }

    private static IEnumerable<(string[] Selectors, string[] Declarations)> CodeRules()
    {
        yield return (
            [$"{Scope} code[class*=\"language-\"]", $"{Scope} pre[class*=\"language-\"]"],
            ["color: var(--osirion-code-text)", "text-shadow: none"]);
        yield return (
            [$"{Scope} pre[class*=\"language-\"]", $"{Scope} :not(pre) > code[class*=\"language-\"]"],
            ["background: var(--osirion-code-background)"]);
        foreach (var (token, classes) in PrismRules)
        {
            yield return (classes.Select(name => $"{Scope} .token.{name}").ToArray(), [$"color: var(--osirion-{token})"]);
        }

        // Prism's themes fade namespaces, which would take their colour below the contrast target.
        yield return ([$"{Scope} .token.namespace"], ["opacity: 1"]);
    }
}
