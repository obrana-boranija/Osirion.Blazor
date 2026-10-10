using Osirion.Blazor.Core.Utils;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Utils;

public class LocalUrlTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/blog")]
    [InlineData("/blog/post?page=2&q=c%23")]
    [InlineData("/docs#intro")]
    [InlineData("/a\\b")]
    public void IsLocalPath_RootRelativePath_ReturnsTrue(string url)
    {
        LocalUrl.IsLocalPath(url).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("blog")]
    [InlineData("~/blog")]
    [InlineData("//evil.example")]
    [InlineData("//evil.example/path")]
    [InlineData("/\\evil.example")]
    [InlineData("\\\\evil.example")]
    [InlineData("https://evil.example")]
    [InlineData("http://localhost/blog")]
    [InlineData("javascript:alert(1)")]
    [InlineData("/\t/evil.example")]
    [InlineData("/\r\n/evil.example")]
    [InlineData("/blog\0")]
    [InlineData("/blog\u007F")]
    public void IsLocalPath_NonLocalOrUnsafeValue_ReturnsFalse(string? url)
    {
        LocalUrl.IsLocalPath(url).ShouldBeFalse();
    }

    [Fact]
    public void GetSafeLocalPath_LocalPath_ReturnsInput()
    {
        LocalUrl.GetSafeLocalPath("/blog?page=2").ShouldBe("/blog?page=2");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("//evil.example")]
    [InlineData("https://evil.example")]
    public void GetSafeLocalPath_RejectedValue_ReturnsDefaultPath(string? url)
    {
        LocalUrl.GetSafeLocalPath(url).ShouldBe(LocalUrl.DefaultPath);
    }

    [Fact]
    public void GetSafeLocalPath_RejectedValue_ReturnsCustomFallback()
    {
        LocalUrl.GetSafeLocalPath("//evil.example", "/osirion").ShouldBe("/osirion");
    }

    [Theory]
    [InlineData("")]
    [InlineData("//evil.example")]
    [InlineData("https://evil.example")]
    public void GetSafeLocalPath_NonLocalFallback_Throws(string fallback)
    {
        Should.Throw<ArgumentException>(() => LocalUrl.GetSafeLocalPath("/blog", fallback));
    }
}
