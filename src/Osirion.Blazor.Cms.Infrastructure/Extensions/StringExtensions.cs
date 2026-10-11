using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for string operations
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to a URL-friendly slug.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="suffixLength">Length of random letter suffix (0 for no suffix).</param>
    /// <returns>URL-friendly slug starting with '/' and proper formatting.</returns>
    public static string ToUrlSlug(this string input, uint suffixLength = 0)
    {
        if (string.IsNullOrWhiteSpace(input))
            return UntitledSlug(suffixLength);

        var slugBuilder = CollectSlugCharacters(input, out var hasValidChars);
        EnsureLetterAtSecondPosition(slugBuilder);
        AppendSlugEnding(slugBuilder, suffixLength);

        // If nothing valid was found, return a default
        if (!hasValidChars)
            return UntitledSlug(suffixLength);

        return slugBuilder.ToString();
    }

    private static string UntitledSlug(uint suffixLength) =>
        "/" + (suffixLength > 0 ? "untitled-" + GenerateRandomLetters(suffixLength) : "untitled");

    /// <summary>
    /// Keeps ASCII letters and digits and turns runs of word separators into one hyphen, in a single pass.
    /// </summary>
    private static StringBuilder CollectSlugCharacters(string input, out bool hasLetters)
    {
        var slugBuilder = new StringBuilder(input.Length + 10);
        var previousWasHyphen = false;
        hasLetters = false;

        foreach (char c in input.ToLowerInvariant())
        {
            if (char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c))
            {
                slugBuilder.Append(c);
                previousWasHyphen = false;
                hasLetters |= char.IsAsciiLetterLower(c);
            }
            else if (IsWordSeparator(c) && !previousWasHyphen && slugBuilder.Length > 1)
            {
                slugBuilder.Append('-');
                previousWasHyphen = true;
            }
        }

        // Remove trailing hyphen if exists
        if (previousWasHyphen && slugBuilder.Length > 1)
        {
            slugBuilder.Length--;
        }

        return slugBuilder;
    }

    private static bool IsWordSeparator(char c) => c is ' ' or '-' or '_';

    // The index is 1 because the slug used to start with a '/' that is no longer added.
    private static void EnsureLetterAtSecondPosition(StringBuilder slugBuilder)
    {
        if (slugBuilder.Length > 1 && char.IsLetter(slugBuilder[1]))
            return;

        slugBuilder.Insert(1, 'a');
        if (slugBuilder.Length > 2 && !char.IsLetter(slugBuilder[2]))
            slugBuilder.Insert(2, '-');
    }

    private static void AppendSlugEnding(StringBuilder slugBuilder, uint suffixLength)
    {
        if (suffixLength > 0)
        {
            if (!char.IsLetter(slugBuilder[^1]))
                slugBuilder.Append('-');

            slugBuilder.Append(GenerateRandomLetters(suffixLength));
            return;
        }

        // If no suffix, ensure slug ends with a letter
        if (slugBuilder.Length > 1 && !char.IsLetter(slugBuilder[^1]))
            slugBuilder.Append('z');
    }

    [ThreadStatic]
    private static Random _random = default!;

    /// <summary>
    /// Generates a random string of lowercase letters
    /// </summary>
    /// <param name="length">The desired length of the random string</param>
    /// <returns>A string of random lowercase letters</returns>
    private static string GenerateRandomLetters(uint length)
    {
        // Use ThreadStatic Random for better performance in multithreaded environments
        _random ??= new Random();

        const string chars = "abcdefghijklmnopqrstuvwxyz";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[_random.Next(chars.Length)];
        }

        return new string(result);
    }
}
