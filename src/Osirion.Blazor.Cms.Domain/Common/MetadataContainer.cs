namespace Osirion.Blazor.Cms.Domain.Common;

/// <summary>
/// Provides standard metadata handling functionality for entities
/// </summary>
public class MetadataContainer
{
    private readonly Dictionary<string, object> _metadata = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the metadata dictionary for read access
    /// </summary>
    public IReadOnlyDictionary<string, object> Values => _metadata;

    /// <summary>
    /// Gets a strongly-typed metadata value
    /// </summary>
    /// <typeparam name="T">The expected type of the value</typeparam>
    /// <param name="key">The metadata key</param>
    /// <param name="defaultValue">The default value to return if key is not found or type doesn't match</param>
    /// <returns>The metadata value or default</returns>
    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        if (_metadata.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }

            // Try to convert if possible
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Sets a metadata value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The metadata key</param>
    /// <param name="value">The value to set</param>
    /// <exception cref="ArgumentException">Thrown when key is null or empty</exception>
    public void SetValue<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));
        }

        if (value == null)
        {
            _metadata.Remove(key);
        }
        else
        {
            _metadata[key] = value;
        }
    }

    /// <summary>
    /// Adds all entries from a dictionary to the metadata
    /// </summary>
    /// <param name="values">Dictionary of values to add</param>
    public void AddRange(IDictionary<string, object> values)
    {
        if (values == null)
            return;

        foreach (var kvp in values)
        {
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                _metadata[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// Removes all metadata entries
    /// </summary>
    public void Clear()
    {
        _metadata.Clear();
    }

    /// <summary>
    /// Checks if a key exists in the metadata
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if key exists, false otherwise</returns>
    public bool ContainsKey(string key)
    {
        return _metadata.ContainsKey(key);
    }

    /// <summary>
    /// Creates a deep clone of the metadata container
    /// </summary>
    /// <returns>A new metadata container with the same values</returns>
    public MetadataContainer Clone()
    {
        var clone = new MetadataContainer();
        foreach (var kvp in _metadata)
        {
            clone._metadata[kvp.Key] = kvp.Value;
        }
        return clone;
    }
}