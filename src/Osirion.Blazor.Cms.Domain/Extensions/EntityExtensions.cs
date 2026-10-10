namespace Osirion.Blazor.Cms.Domain.Extensions;

/// <summary>
/// Extension methods for domain entities
/// </summary>
public static class EntityExtensions
{
    private const string UntitledSlug = "untitled";

    // Letters that Unicode decomposition does not reduce to an ASCII base letter, plus Serbian/Russian Cyrillic.
    // Letters shared by both Cyrillic alphabets follow the Serbian Latin form (ж z, ч c, ш s, х h, ц c).
    private static readonly Dictionary<char, string> Transliterations = new()
    {
        ['đ'] = "dj", ['ð'] = "d", ['ł'] = "l", ['ø'] = "o", ['ß'] = "ss", ['æ'] = "ae", ['œ'] = "oe", ['þ'] = "th", ['ı'] = "i",
        ['а'] = "a", ['б'] = "b", ['в'] = "v", ['г'] = "g", ['д'] = "d", ['ђ'] = "dj", ['е'] = "e", ['ж'] = "z",
        ['з'] = "z", ['и'] = "i", ['ј'] = "j", ['к'] = "k", ['л'] = "l", ['љ'] = "lj", ['м'] = "m", ['н'] = "n",
        ['њ'] = "nj", ['о'] = "o", ['п'] = "p", ['р'] = "r", ['с'] = "s", ['т'] = "t", ['ћ'] = "c", ['у'] = "u",
        ['ф'] = "f", ['х'] = "h", ['ц'] = "c", ['ч'] = "c", ['џ'] = "dz", ['ш'] = "s",
        ['ё'] = "e", ['й'] = "j", ['щ'] = "sc", ['ъ'] = "", ['ы'] = "y", ['ь'] = "", ['э'] = "e", ['ю'] = "ju", ['я'] = "ja",
        ['ѓ'] = "g", ['ќ'] = "k", ['ѕ'] = "dz", ['є'] = "e", ['і'] = "i", ['ї'] = "ji", ['ґ'] = "g"
    };

    /// <summary>
    /// Generates a URL-friendly ASCII slug from text. Diacritics are removed, Serbian Latin (for example
    /// <c>đ</c> becomes <c>dj</c>) and Serbian/Russian Cyrillic are transliterated. Text that has letters or digits
    /// but yields no ASCII characters (for example CJK) gets a stable short hash of the text instead of a shared
    /// placeholder, so different titles do not collide.
    /// </summary>
    public static string GenerateSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return UntitledSlug;

        var slug = Transliterate(text.Trim().ToLowerInvariant());

        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        if (slug.Length > 0)
            return slug;

        return text.Any(char.IsLetterOrDigit) ? CreateHashSlug(text) : UntitledSlug;
    }

    private static string Transliterate(string lowercaseText)
    {
        var builder = new System.Text.StringBuilder(lowercaseText.Length);
        foreach (var character in lowercaseText)
        {
            if (Transliterations.TryGetValue(character, out var replacement))
                builder.Append(replacement);
            else
                builder.Append(character);
        }

        var decomposed = builder.ToString().Normalize(System.Text.NormalizationForm.FormD);
        var result = new System.Text.StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) != System.Globalization.UnicodeCategory.NonSpacingMark)
                result.Append(character);
        }

        return result.ToString();
    }

    private static string CreateHashSlug(string text)
    {
        var normalized = text.Trim().Normalize(System.Text.NormalizationForm.FormC);
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(normalized));
        return "t-" + Convert.ToHexStringLower(hash.AsSpan(0, 4));
    }

    /// <summary>
    /// Validates if a string is a valid slug
    /// </summary>
    public static bool IsValidSlug(this string slug)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }

    /// <summary>
    /// Escapes a string for YAML
    /// </summary>
    public static string EscapeYamlString(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
