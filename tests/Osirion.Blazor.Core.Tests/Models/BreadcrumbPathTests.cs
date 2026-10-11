using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Models;

public class BreadcrumbPathTests
{
    [Fact]
    public void BreadcrumbPath_NestedPath_BuildsCumulativeSegments()
    {
        var path = new BreadcrumbPath("/docs/guides/setup/");

        path.FullPath.ShouldBe("docs/guides/setup");
        path.Segments.Select(segment => (segment.Name, segment.Path, segment.Index, segment.IsLast)).ShouldBe(
        [
            ("docs", "docs", 0, false),
            ("guides", "docs/guides", 1, false),
            ("setup", "docs/guides/setup", 2, true)
        ]);
    }

    [Fact]
    public void BreadcrumbPath_RepeatedSeparators_SkipsEmptySegments()
    {
        var path = new BreadcrumbPath("a//b");

        path.FullPath.ShouldBe("a//b");
        path.Segments.Select(segment => segment.Path).ShouldBe(["a", "a/b"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("/")]
    [InlineData("   ")]
    public void BreadcrumbPath_EmptyPath_HasNoSegments(string? value)
    {
        var path = new BreadcrumbPath(value!);

        path.Segments.ShouldBeEmpty();
    }

    [Fact]
    public void BreadcrumbPath_NullPath_HasEmptyFullPath()
    {
        new BreadcrumbPath(null!).FullPath.ShouldBe(string.Empty);
    }

    [Fact]
    public void FromSegments_JoinsTheSegmentsIntoOnePath()
    {
        var path = BreadcrumbPath.FromSegments(["blog", "2026", "post"]);

        path.FullPath.ShouldBe("blog/2026/post");
        path.Segments.Count.ShouldBe(3);
        path.Segments[^1].IsLast.ShouldBeTrue();
    }

    [Fact]
    public void FromSegments_NoSegments_HasNoSegments()
    {
        BreadcrumbPath.FromSegments([]).Segments.ShouldBeEmpty();
    }
}
