using System.Text;

namespace Osirion.Blazor.Components;

/// <summary>
/// Utility for parsing paths into breadcrumb segments with accumulated paths
/// </summary>
public class BreadcrumbPath
{
    /// <summary>
    /// The original full path
    /// </summary>
    public string FullPath { get; }

    /// <summary>
    /// Collection of breadcrumb segments with cumulative paths
    /// </summary>
    public IReadOnlyList<BreadcrumbSegment> Segments { get; }

    /// <summary>
    /// Creates a new instance of BreadcrumbPath from a path string
    /// </summary>
    /// <param name="path">The path to parse</param>
    public BreadcrumbPath(string path)
    {
        FullPath = path?.Trim('/') ?? string.Empty;
        Segments = ParsePath(FullPath);
    }

    /// <summary>
    /// Creates a BreadcrumbPath from a collection of segments
    /// </summary>
    /// <param name="segments">The segments to combine</param>
    /// <returns>A new BreadcrumbPath instance</returns>
    public static BreadcrumbPath FromSegments(IEnumerable<string> segments)
    {
        var path = string.Join('/', segments);
        return new BreadcrumbPath(path);
    }

    /// <summary>
    /// Parses a path into breadcrumb segments
    /// </summary>
    private static IReadOnlyList<BreadcrumbSegment> ParsePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Array.Empty<BreadcrumbSegment>();

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<BreadcrumbSegment>(segments.Length);

        var cumulativePath = new StringBuilder();

        for (int i = 0; i < segments.Length; i++)
        {
            if (i > 0)
                cumulativePath.Append('/');

            cumulativePath.Append(segments[i]);

            result.Add(new BreadcrumbSegment(
                name: segments[i],
                path: cumulativePath.ToString(),
                index: i,
                isLast: i == segments.Length - 1
            ));
        }

        return result;
    }
}