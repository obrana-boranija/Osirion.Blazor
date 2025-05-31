namespace Osirion.Blazor.Cms.Admin;

public static class QueryHelpers
{
    public static Dictionary<string, string[]> ParseQuery(string queryString)
    {
        var result = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(queryString))
            return result;

        int scanIndex = 0;
        if (queryString[0] == '?')
            scanIndex = 1;

        int textLength = queryString.Length;

        while (scanIndex < textLength)
        {
            int delimiterIndex = queryString.IndexOf('&', scanIndex);
            if (delimiterIndex == -1)
                delimiterIndex = textLength;

            int equalIndex = queryString.IndexOf('=', scanIndex);
            if (equalIndex == -1 || equalIndex > delimiterIndex)
                equalIndex = delimiterIndex;

            string name = queryString.Substring(scanIndex, equalIndex - scanIndex);
            string value = equalIndex < delimiterIndex
                ? queryString.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1)
                : string.Empty;

            name = Uri.UnescapeDataString(name.Replace('+', ' '));
            value = Uri.UnescapeDataString(value.Replace('+', ' '));

            if (result.TryGetValue(name, out var existingValues))
            {
                var newValues = new string[existingValues.Length + 1];
                Array.Copy(existingValues, newValues, existingValues.Length);
                newValues[existingValues.Length] = value;
                result[name] = newValues;
            }
            else
            {
                result[name] = new string[] { value };
            }

            scanIndex = delimiterIndex + 1;
        }

        return result;
    }

    // Fix the extension method to match the expected signature
    public static bool TryGetValue(this Dictionary<string, string[]> query, string key, out string code)
    {
        if (query.TryGetValue(key, out var values) && values.Length > 0)
        {
            code = values[0];
            return true;
        }

        code = string.Empty;
        return false;
    }
}
