using System.Globalization;

namespace Osirion.Blazor.Palettes;

/// <summary>
/// An opaque sRGB colour with the arithmetic the palettes need: WCAG 2 contrast, mixing in OKLab, and moving a colour
/// lighter or darker until it reaches a contrast ratio.
/// </summary>
internal readonly record struct Rgb(double R, double G, double B)
{
    /// <summary>Parses <c>#rgb</c>, <c>#rrggbb</c> or <c>#rrggbbaa</c>; a colour with alpha is composited over <paramref name="backdrop"/>.</summary>
    public static Rgb Parse(string hex, Rgb? backdrop = null)
    {
        var value = hex.TrimStart('#');
        if (value.Length == 3)
        {
            value = string.Concat(value.Select(c => new string(c, 2)));
        }

        if (value.Length is not (6 or 8))
        {
            throw new FormatException($"'{hex}' is not a #rrggbb or #rrggbbaa colour.");
        }

        var colour = new Rgb(Channel(value, 0), Channel(value, 2), Channel(value, 4));
        if (value.Length == 6)
        {
            return colour;
        }

        var alpha = Channel(value, 6);
        var under = backdrop ?? throw new FormatException($"'{hex}' has alpha but no backdrop to composite it over.");
        return new Rgb(
            colour.R * alpha + under.R * (1 - alpha),
            colour.G * alpha + under.G * (1 - alpha),
            colour.B * alpha + under.B * (1 - alpha));
    }

    private static double Channel(string value, int start) =>
        int.Parse(value.AsSpan(start, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;

    /// <summary>Gets the colour as lower-case <c>#rrggbb</c>.</summary>
    public string ToHex() => $"#{Byte(R):x2}{Byte(G):x2}{Byte(B):x2}";

    private static int Byte(double channel) => (int)Math.Round(Math.Clamp(channel, 0, 1) * 255);

    /// <summary>Gets the relative luminance as WCAG 2 defines it.</summary>
    public double Luminance
    {
        get
        {
            // Rounded to bytes first, so the ratio is the one a browser shows for the written hex value.
            var exact = Parse(ToHex());
            return 0.2126 * Linear(exact.R) + 0.7152 * Linear(exact.G) + 0.0722 * Linear(exact.B);
        }
    }

    private static double Linear(double c) => c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);

    private static double Gamma(double c) => c <= 0.0031308 ? 12.92 * c : 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;

    /// <summary>Gets the WCAG 2 contrast ratio between two colours, from 1 to 21.</summary>
    public static double Contrast(Rgb a, Rgb b)
    {
        var (lighter, darker) = a.Luminance >= b.Luminance ? (a.Luminance, b.Luminance) : (b.Luminance, a.Luminance);
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>Mixes two colours in OKLab: 0 gives <paramref name="from"/>, 1 gives <paramref name="to"/>; values outside 0 to 1 extrapolate.</summary>
    public static Rgb Mix(Rgb from, Rgb to, double amount)
    {
        var a = from.ToOklab();
        var b = to.ToOklab();
        return FromOklab(
            a.L + (b.L - a.L) * amount,
            a.A + (b.A - a.A) * amount,
            a.B + (b.B - a.B) * amount);
    }

    /// <summary>
    /// Returns this colour when it already reaches <paramref name="minimum"/> against <paramref name="background"/>;
    /// otherwise moves its OKLab lightness away from the background, keeping the hue, until it does.
    /// </summary>
    public Rgb EnsureContrast(Rgb background, double minimum)
    {
        if (Contrast(this, background) >= minimum)
        {
            return this;
        }

        var lab = ToOklab();
        var direction = background.Luminance > 0.18 ? -1 : 1;
        for (var lightness = lab.L; lightness is >= 0 and <= 1; lightness += direction * 0.005)
        {
            var candidate = FromOklab(lightness, lab.A, lab.B);
            if (Contrast(candidate, background) >= minimum)
            {
                return candidate;
            }
        }

        return direction < 0 ? new Rgb(0, 0, 0) : new Rgb(1, 1, 1);
    }

    private (double L, double A, double B) ToOklab()
    {
        var r = Linear(R);
        var g = Linear(G);
        var b = Linear(B);
        var l = Math.Cbrt(0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b);
        var m = Math.Cbrt(0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b);
        var s = Math.Cbrt(0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b);
        return (
            0.2104542553 * l + 0.7936177850 * m - 0.0040720468 * s,
            1.9779984951 * l - 2.4285922050 * m + 0.4505937099 * s,
            0.0259040371 * l + 0.7827717662 * m - 0.8086757660 * s);
    }

    private static Rgb FromOklab(double lightness, double a, double b)
    {
        var l = Math.Pow(lightness + 0.3963377774 * a + 0.2158037573 * b, 3);
        var m = Math.Pow(lightness - 0.1055613458 * a - 0.0638541728 * b, 3);
        var s = Math.Pow(lightness - 0.0894841775 * a - 1.2914855480 * b, 3);
        return new Rgb(
            Gamma(Math.Clamp(4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s, 0, 1)),
            Gamma(Math.Clamp(-1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s, 0, 1)),
            Gamma(Math.Clamp(-0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s, 0, 1)));
    }
}
