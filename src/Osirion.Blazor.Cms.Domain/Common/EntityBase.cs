namespace Osirion.Blazor.Cms.Domain.Common;

/// <summary>
/// Base class for domain entities that require metadata
/// </summary>
public abstract class EntityBase<TId> : Entity<TId> where TId : notnull
{
    private readonly MetadataContainer _metadata = new();

    /// <summary>
    /// Gets the metadata dictionary as read-only
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata => _metadata.Values;

    /// <summary>
    /// Gets a strongly-typed metadata value
    /// </summary>
    /// <typeparam name="T">Type of value to retrieve</typeparam>
    /// <param name="key">Metadata key</param>
    /// <param name="defaultValue">Default value if key not found</param>
    /// <returns>The value or default</returns>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        return _metadata.GetValue(key, defaultValue);
    }

    /// <summary>
    /// Sets a metadata value
    /// </summary>
    /// <typeparam name="T">Type of value to set</typeparam>
    /// <param name="key">Metadata key</param>
    /// <param name="value">Value to set</param>
    public void SetMetadata<T>(string key, T value)
    {
        _metadata.SetValue(key, value);
    }

    /// <summary>
    /// Gets the metadata container for derived classes
    /// </summary>
    protected MetadataContainer GetMetadataContainer() => _metadata;

    /// <summary>
    /// Copies metadata to the specified metadata container
    /// </summary>
    protected void CopyMetadataTo(MetadataContainer targetContainer)
    {
        foreach (var kvp in _metadata.Values)
        {
            targetContainer.SetValue(kvp.Key, kvp.Value);
        }
    }
}