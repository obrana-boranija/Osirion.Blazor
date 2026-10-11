using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Osirion.Blazor.Core.Utils;

/// <summary>
/// Makes author-supplied JSON-LD safe to write inside a <c>&lt;script type="application/ld+json"&gt;</c> element.
/// </summary>
/// <remarks>
/// The value is parsed as strict JSON (no comments, no trailing commas) and written back with the default
/// System.Text.Json encoder, which escapes <c>&lt;</c>, <c>&gt;</c>, <c>&amp;</c> and quotes as <c>\uXXXX</c>.
/// The result therefore cannot contain <c>&lt;/script&gt;</c> or <c>&lt;!--</c>, which would end the script
/// element or change how the browser parses it, and it still means the same thing to every JSON-LD consumer.
/// </remarks>
public static class JsonLdNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Parses <paramref name="jsonLd"/> and re-serializes it with HTML-sensitive characters escaped.
    /// </summary>
    /// <param name="jsonLd">The JSON-LD text, typically from content front matter.</param>
    /// <param name="normalized">The escaped JSON when the method returns <see langword="true"/>.</param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="jsonLd"/> is a JSON object or array;
    /// <see langword="false"/> when it is empty, not valid JSON, a bare value such as a string or number, or
    /// holds text that is not valid Unicode, such as half of a surrogate pair.
    /// </returns>
    public static bool TryNormalize(string? jsonLd, [NotNullWhen(true)] out string? normalized)
    {
        normalized = null;

        if (string.IsNullOrWhiteSpace(jsonLd))
        {
            return false;
        }

        try
        {
            var node = JsonNode.Parse(jsonLd);
            if (node is not (JsonObject or JsonArray))
            {
                return false;
            }

            // String values are decoded only when written, so invalid text surfaces here rather than in Parse.
            normalized = node.ToJsonString(SerializerOptions);
            return true;
        }
        catch (Exception exception) when (IsInvalidJsonText(exception))
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether an exception from System.Text.Json means the input text cannot be used as JSON:
    /// a syntax error (<see cref="JsonException"/>), a raw lone surrogate that cannot be transcoded to UTF-8
    /// (<see cref="ArgumentException"/>) or an escaped lone surrogate that cannot be decoded
    /// (<see cref="InvalidOperationException"/>).
    /// </summary>
    private static bool IsInvalidJsonText(Exception exception) =>
        exception is JsonException or ArgumentException or InvalidOperationException;
}
