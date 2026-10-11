using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubTreeSnapshotTests
{
    private readonly GitHubTreeSnapshot _snapshot = new(new GitHubTree
    {
        Sha = "root-sha",
        Entries =
        [
            new GitHubTreeEntry { Path = "README.md", Type = "blob", Sha = "s-readme" },
            new GitHubTreeEntry { Path = "content", Type = "tree", Sha = "s-content" },
            new GitHubTreeEntry { Path = "content/_index.md", Type = "blob", Sha = "s-index" },
            new GitHubTreeEntry { Path = "content/blog", Type = "tree", Sha = "s-blog" },
            new GitHubTreeEntry { Path = "content/blog/post.md", Type = "blob", Sha = "s-post" },
            new GitHubTreeEntry { Path = "vendor/lib", Type = "commit", Sha = "s-submodule" }
        ]
    });

    [Fact]
    public void ListChildren_ForTheRoot_ReturnsTopLevelFilesAndDirectories()
    {
        // Act
        var children = _snapshot.ListChildren(string.Empty);

        // Assert
        children.Select(c => c.Path).ShouldBe(["README.md", "content"]);
        children[0].ShouldSatisfyAllConditions(
            c => c.Name.ShouldBe("README.md"),
            c => c.Sha.ShouldBe("s-readme"),
            c => c.Type.ShouldBe("file"));
        children[1].ShouldSatisfyAllConditions(
            c => c.Name.ShouldBe("content"),
            c => c.Sha.ShouldBe("s-content"),
            c => c.Type.ShouldBe("dir"));
    }

    [Theory]
    [InlineData("content")]
    [InlineData("/content/")]
    public void ListChildren_ForADirectory_TrimsSlashesAndReturnsItsDirectChildren(string path)
    {
        // Act
        var children = _snapshot.ListChildren(path);

        // Assert
        children.Select(c => (c.Name, c.Path, c.Type)).ShouldBe(
        [
            ("_index.md", "content/_index.md", "file"),
            ("blog", "content/blog", "dir")
        ]);
    }

    [Fact]
    public void ListChildren_ForANestedDirectory_ReturnsItsFiles()
    {
        // Act
        var children = _snapshot.ListChildren("content/blog");

        // Assert
        children.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            c => c.Name.ShouldBe("post.md"),
            c => c.Path.ShouldBe("content/blog/post.md"),
            c => c.Sha.ShouldBe("s-post"));
    }

    [Theory]
    [InlineData("vendor")]
    [InlineData("missing")]
    [InlineData("content/blog/post.md")]
    public void ListChildren_ForAPathWithoutChildren_ReturnsAnEmptyList(string path)
    {
        // Act
        var children = _snapshot.ListChildren(path);

        // Assert
        children.ShouldBeEmpty();
    }

    [Fact]
    public void ListChildren_ReturnsACopy_SoCallersCannotChangeTheSnapshot()
    {
        // Arrange
        var first = _snapshot.ListChildren("content");

        // Act
        first.Clear();

        // Assert
        _snapshot.ListChildren("content").Count.ShouldBe(2);
    }

    [Theory]
    [InlineData("README.md", true)]
    [InlineData("content/blog/post.md", true)]
    [InlineData("content/blog", false)]
    [InlineData("vendor/lib", false)]
    [InlineData("Content/Blog/Post.md", false)]
    [InlineData("missing.md", false)]
    public void ContainsFile_ReportsOnlyBlobsByExactPath(string path, bool expected)
    {
        // Act
        var contains = _snapshot.ContainsFile(path);

        // Assert
        contains.ShouldBe(expected);
    }

    [Fact]
    public void Constructor_WithAnEmptyTree_ListsNothing()
    {
        // Arrange
        var snapshot = new GitHubTreeSnapshot(new GitHubTree());

        // Act
        var children = snapshot.ListChildren(string.Empty);

        // Assert
        children.ShouldBeEmpty();
        snapshot.ContainsFile("README.md").ShouldBeFalse();
    }
}
