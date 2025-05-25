namespace Osirion.Blazor.Components;

/// <summary>
/// Represents a segment in a breadcrumb path
/// </summary>
public class BreadcrumbSegment
{
    /// <summary>
    /// The name of this segment
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The cumulative path up to and including this segment
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The index of this segment in the path
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Whether this is the last segment in the path
    /// </summary>
    public bool IsLast { get; }

    /// <summary>
    /// Creates a new breadcrumb segment
    /// </summary>
    /// <param name="name">The segment name</param>
    /// <param name="path">The cumulative path</param>
    /// <param name="index">The index in the path</param>
    /// <param name="isLast">Whether this is the last segment</param>
    public BreadcrumbSegment(string name, string path, int index, bool isLast)
    {
        Name = name;
        Path = path;
        Index = index;
        IsLast = isLast;
    }
}