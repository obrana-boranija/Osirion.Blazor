namespace Osirion.Blazor.Palettes;

/// <summary>
/// The colors one palette variant is built from. Every value is a <c>#rrggbb</c> or <c>#rrggbbaa</c> hex color; a
/// color with alpha is composited over <see cref="Background"/>, so the stylesheet only holds opaque colors whose
/// contrast can be checked.
/// </summary>
internal sealed record PaletteSpec
{
    /// <summary>Gets the palette id, as written in <c>data-osirion-palette</c> (for example <c>commit</c>).</summary>
    public required string Palette { get; init; }

    /// <summary>Gets <c>light</c> or <c>dark</c>, the <c>data-osirion-theme</c> value this variant applies to.</summary>
    public required string Mode { get; init; }

    /// <summary>Gets further palette ids that use this variant for the same mode (Studio Blue's dark mode is Studio Dark).</summary>
    public IReadOnlyList<string> AlsoFor { get; init; } = [];

    /// <summary>Gets the lowest contrast ratio for text; 4.5 is WCAG 2.2 AA, the high-contrast palette asks for 7 (AAA).</summary>
    public double TextContrast { get; init; } = 4.5;

    public required string Background { get; init; }
    public required string Surface { get; init; }
    public required string SurfaceAlt { get; init; }
    public required string Muted { get; init; }
    public required string Border { get; init; }
    public required string BorderStrong { get; init; }
    public required string Text { get; init; }
    public required string TextSecondary { get; init; }
    public required string TextMuted { get; init; }
    public required string TextDisabled { get; init; }
    public required string Accent { get; init; }
    public required string AccentHover { get; init; }
    public required string AccentText { get; init; }
    public required string Link { get; init; }
    public required string Focus { get; init; }
    public required string NavActiveBackground { get; init; }
    public required string NavActiveText { get; init; }
    public string? InputBackground { get; init; }

    public required string Success { get; init; }
    public required string Warning { get; init; }
    public required string Danger { get; init; }
    public required string Info { get; init; }
    public string? SuccessBackground { get; init; }
    public string? WarningBackground { get; init; }
    public string? DangerBackground { get; init; }
    public string? InfoBackground { get; init; }

    public required CodeColors Code { get; init; }

    /// <summary>Gets tokens written as given, after the generated ones (for example the Studio Blue menu frame).</summary>
    public IReadOnlyDictionary<string, string> Extra { get; init; } = new Dictionary<string, string>();
}

/// <summary>Syntax highlighting colors, mapped onto Prism's token classes.</summary>
internal sealed record CodeColors
{
    public required string Background { get; init; }
    public required string Text { get; init; }
    public required string Comment { get; init; }
    public required string Keyword { get; init; }
    public required string String { get; init; }
    public required string Number { get; init; }
    public required string Function { get; init; }
    public required string Type { get; init; }
    public required string Variable { get; init; }
    public required string Tag { get; init; }
    public required string Attribute { get; init; }
    public required string Regex { get; init; }
    public required string Inserted { get; init; }
    public required string Deleted { get; init; }
}
