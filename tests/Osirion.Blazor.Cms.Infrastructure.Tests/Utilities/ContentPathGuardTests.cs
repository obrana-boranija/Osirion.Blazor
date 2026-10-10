using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

public class ContentPathGuardTests
{
    private static readonly string SiteRoot = Path.Combine(Path.GetTempPath(), "osirion-guard-site");
    private static readonly string ContentRoot = Path.Combine(SiteRoot, "content");

    public static TheoryData<string> PathsOutsideTheRoot()
    {
        var paths = new TheoryData<string>
        {
            "../outside.md",
            "blog/../../outside.md",
            "..\\outside.md",
            "blog/./post.md",
            "..",
            "/etc/passwd",
            Path.Combine(SiteRoot, "appsettings.json"),
            "",
            "   ",
            "blog/post\0.md"
        };

        if (OperatingSystem.IsWindows())
        {
            paths.Add("C:\\Windows\\win.ini");
            paths.Add("C:outside.md");
            paths.Add("\\\\server\\share\\outside.md");
            paths.Add("\\outside.md");
        }

        return paths;
    }

    [Theory]
    [MemberData(nameof(PathsOutsideTheRoot))]
    public void ResolveUnderRoot_PathNotStrictlyInsideTheRoot_ThrowsContentValidationExceptionForPath(string relativePath)
    {
        var exception = Should.Throw<ContentValidationException>(() => ContentPathGuard.ResolveUnderRoot(ContentRoot, relativePath));

        exception.Errors.ShouldContainKey("Path");
    }

    [Theory]
    [InlineData("blog/post.md")]
    [InlineData("blog/sub/post.md")]
    [InlineData("blog\\post.md")]
    [InlineData("v1..2.md")]
    [InlineData(".gitkeep")]
    [InlineData("blog/članak.md")]
    public void ResolveUnderRoot_PathInsideTheRoot_ReturnsTheFullPathUnderTheRoot(string relativePath)
    {
        var fullPath = ContentPathGuard.ResolveUnderRoot(ContentRoot, relativePath);

        fullPath.ShouldStartWith(Path.GetFullPath(ContentRoot) + Path.DirectorySeparatorChar);
        fullPath.ShouldBe(Path.GetFullPath(Path.Combine(ContentRoot, relativePath)));
    }

    [Fact]
    public void ResolveUnderRoot_RootWithTrailingSeparator_AcceptsPathInsideTheRoot()
    {
        var fullPath = ContentPathGuard.ResolveUnderRoot(ContentRoot + Path.DirectorySeparatorChar, "post.md");

        fullPath.ShouldBe(Path.Combine(Path.GetFullPath(ContentRoot), "post.md"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void ResolveUnderRoot_RootNotConfigured_ThrowsInvalidOperationException(string rootPath)
    {
        Should.Throw<InvalidOperationException>(() => ContentPathGuard.ResolveUnderRoot(rootPath, "post.md"));
    }

    [Fact]
    public void IsUnderRoot_FileInsideTheRoot_ReturnsTrue()
    {
        ContentPathGuard.IsUnderRoot(ContentRoot, Path.Combine(ContentRoot, "blog", "post.md")).ShouldBeTrue();
    }

    [Fact]
    public void IsUnderRoot_SiblingDirectorySharingTheRootPrefix_ReturnsFalse()
    {
        ContentPathGuard.IsUnderRoot(ContentRoot, Path.Combine(SiteRoot, "content-evil", "post.md")).ShouldBeFalse();
    }

    [Fact]
    public void IsUnderRoot_PathThatTraversesOutOfTheRoot_ReturnsFalse()
    {
        ContentPathGuard.IsUnderRoot(ContentRoot, Path.Combine(ContentRoot, "..", "appsettings.json")).ShouldBeFalse();
    }

    [Fact]
    public void IsUnderRoot_TheRootItself_ReturnsFalse()
    {
        ContentPathGuard.IsUnderRoot(ContentRoot, ContentRoot + Path.DirectorySeparatorChar).ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("post\0.md")]
    public void IsUnderRoot_EmptyOrNullCharacterPath_ReturnsFalse(string fullPath)
    {
        ContentPathGuard.IsUnderRoot(ContentRoot, fullPath).ShouldBeFalse();
    }

    [Fact]
    public void IsUnderRoot_RootInDifferentCase_IsAcceptedOnlyOnWindows()
    {
        var fullPath = Path.Combine(ContentRoot, "post.md");

        ContentPathGuard.IsUnderRoot(ContentRoot.ToUpperInvariant(), fullPath).ShouldBe(OperatingSystem.IsWindows());
    }

    [Fact]
    public void EnsureUnderRoot_PathOutsideTheRoot_ThrowsContentValidationExceptionForPath()
    {
        var exception = Should.Throw<ContentValidationException>(
            () => ContentPathGuard.EnsureUnderRoot(ContentRoot, Path.Combine(SiteRoot, "appsettings.json")));

        exception.Errors.ShouldContainKey("Path");
    }
}
