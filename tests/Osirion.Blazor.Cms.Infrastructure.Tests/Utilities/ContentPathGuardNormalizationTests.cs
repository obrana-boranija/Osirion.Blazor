using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

public class ContentPathGuardNormalizationTests
{
    private static readonly string ContentRoot = Path.Combine(Path.GetTempPath(), "osirion-guard-normalize", "content");

    // Windows drops trailing dots from a path segment, so "..." normalizes to the root itself, which is not a valid
    // target even though it passed the segment checks. Elsewhere "..." is an ordinary file name inside the root.
    [Fact]
    public void ResolveUnderRoot_SegmentOfThreeDots_IsRefusedOnlyWhereItNormalizesToTheRoot()
    {
        if (OperatingSystem.IsWindows())
        {
            var exception = Should.Throw<ContentValidationException>(() => ContentPathGuard.ResolveUnderRoot(ContentRoot, "..."));
            exception.Errors.ShouldContainKey("Path");
        }
        else
        {
            ContentPathGuard.ResolveUnderRoot(ContentRoot, "...").ShouldBe(Path.Combine(Path.GetFullPath(ContentRoot), "..."));
        }
    }

    [Fact]
    public void EnsureUnderRoot_PathInsideTheRoot_DoesNotThrow()
    {
        Should.NotThrow(() => ContentPathGuard.EnsureUnderRoot(ContentRoot, Path.Combine(ContentRoot, "blog", "post.md")));
    }
}
