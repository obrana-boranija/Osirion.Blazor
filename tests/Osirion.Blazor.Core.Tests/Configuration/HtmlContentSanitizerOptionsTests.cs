using Osirion.Blazor.Core.Configuration;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Configuration;

public class HtmlContentSanitizerOptionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void AllowElement_EmptyAttributeName_Throws(string attributeName)
    {
        var options = new HtmlContentSanitizerOptions();

        var exception = Should.Throw<ArgumentException>(() => options.AllowElement("chart", "type", attributeName));

        exception.ParamName.ShouldBe("attributeNames");
    }

    [Fact]
    public void AllowElement_EmptyAttributeName_LeavesTheElementDisallowed()
    {
        var options = new HtmlContentSanitizerOptions();

        Should.Throw<ArgumentException>(() => options.AllowElement("chart", " "));

        options.AllowedTags.ShouldNotContain("chart");
    }
}
