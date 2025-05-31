namespace Osirion.Blazor.Cms.Domain.Common;

/// <summary>
/// Base class for value objects that need to manage metadata
/// </summary>
public abstract class MetadataValueObject : ValueObject
{
    /// <summary>
    /// Gets the metadata dictionary
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Constructs a metadata value object with an initial metadata dictionary
    /// </summary>
    protected MetadataValueObject(Dictionary<string, object>? initialMetadata = null)
    {
        Metadata = initialMetadata is not null
            ? new Dictionary<string, object>(initialMetadata)
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Gets a metadata value
    /// </summary>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        if (Metadata.TryGetValue(key, out var value))
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
    /// Creates a new version of this object with the specified metadata
    /// </summary>
    protected Dictionary<string, object> CloneMetadataWith(string key, object value)
    {
        var metadata = new Dictionary<string, object>(Metadata);
        metadata[key] = value;
        return metadata;
    }

    /// <summary>
    /// Includes metadata in the equality components
    /// </summary>
    protected IEnumerable<object> GetMetadataEqualityComponents()
    {
        foreach (var kvp in Metadata.OrderBy(x => x.Key))
        {
            yield return $"{kvp.Key}:{kvp.Value}";
        }
    }
}