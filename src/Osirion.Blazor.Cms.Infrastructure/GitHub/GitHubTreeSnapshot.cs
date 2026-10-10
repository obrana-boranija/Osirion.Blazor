using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// An in-memory index of a <see cref="GitHubTree"/> that answers directory listings and file lookups without a request
/// </summary>
internal sealed class GitHubTreeSnapshot
{
    private readonly Dictionary<string, List<GitHubItem>> _childrenByParent = new(StringComparer.Ordinal);
    private readonly HashSet<string> _files = new(StringComparer.Ordinal);

    public GitHubTreeSnapshot(GitHubTree tree)
    {
        foreach (var entry in tree.Entries)
        {
            if (!entry.IsFile && !entry.IsDirectory)
            {
                continue;
            }

            var separator = entry.Path.LastIndexOf('/');
            var parent = separator < 0 ? string.Empty : entry.Path[..separator];
            var name = separator < 0 ? entry.Path : entry.Path[(separator + 1)..];

            if (entry.IsFile)
            {
                _files.Add(entry.Path);
            }

            if (!_childrenByParent.TryGetValue(parent, out var children))
            {
                _childrenByParent[parent] = children = [];
            }

            children.Add(new GitHubItem
            {
                Name = name,
                Path = entry.Path,
                Sha = entry.Sha,
                Type = entry.IsFile ? "file" : "dir"
            });
        }
    }

    /// <summary>Lists the files and directories directly under <paramref name="path"/> (empty for the repository root).</summary>
    public List<GitHubItem> ListChildren(string path)
    {
        return _childrenByParent.TryGetValue(path.Trim('/'), out var children) ? [.. children] : [];
    }

    /// <summary>Returns whether the tree contains a file at <paramref name="path"/>.</summary>
    public bool ContainsFile(string path) => _files.Contains(path);
}
