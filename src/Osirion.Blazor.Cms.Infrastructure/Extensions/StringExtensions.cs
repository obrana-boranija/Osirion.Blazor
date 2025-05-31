using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

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
            return "/" + (suffixLength > 0 ? "untitled-" + GenerateRandomLetters(suffixLength) : "untitled");

        // Use StringBuilder for better performance with multiple string operations
        var slugBuilder = new StringBuilder(input.Length + 10);
        //slugBuilder.Append('/');

        bool previousWasHyphen = false;
        bool hasValidChars = false;

        // Process each character - single pass through the string
        foreach (char c in input.ToLowerInvariant())
        {
            if ('a' <= c && c <= 'z')
            {
                slugBuilder.Append(c);
                previousWasHyphen = false;
                hasValidChars = true;
            }
            else if ('0' <= c && c <= '9')
            {
                slugBuilder.Append(c);
                previousWasHyphen = false;
            }
            else if ((c == ' ' || c == '-' || c == '_') && !previousWasHyphen && slugBuilder.Length > 1)
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

        // Ensure slug starts with a letter after the leading slash
        if (slugBuilder.Length <= 1 || !char.IsLetter(slugBuilder[1]))
        {
            slugBuilder.Insert(1, 'a');
            if (slugBuilder.Length > 2 && !char.IsLetter(slugBuilder[2]))
                slugBuilder.Insert(2, '-');
        }

        // Add random suffix if requested
        if (suffixLength > 0)
        {
            string lastChar = slugBuilder[slugBuilder.Length - 1].ToString();
            if (!char.IsLetter(lastChar, 0))
                slugBuilder.Append('-');

            slugBuilder.Append(GenerateRandomLetters(suffixLength));
        }
        else
        {
            // If no suffix, ensure slug ends with a letter
            if (slugBuilder.Length > 1 && !char.IsLetter(slugBuilder[slugBuilder.Length - 1]))
                slugBuilder.Append('z');
        }

        // If nothing valid was found, return a default
        if (!hasValidChars)
            return "/" + (suffixLength > 0 ? "untitled-" + GenerateRandomLetters(suffixLength) : "untitled");

        return slugBuilder.ToString();
    }

    [ThreadStatic]
    private static Random _random = default!;

    private static string GenerateRandomLetters(uint length)
    {
        // Use ThreadStatic Random for better performance in multithreaded environments
        if (_random is null)
            _random = new Random();

        const string chars = "abcdefghijklmnopqrstuvwxyz";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[_random.Next(chars.Length)];
        }

        return new string(result);
    }
}
