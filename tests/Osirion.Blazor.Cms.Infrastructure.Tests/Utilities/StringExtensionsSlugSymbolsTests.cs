using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

public class StringExtensionsSlugSymbolsTests
{
    [Theory]
    [InlineData("!!!")]
    [InlineData("---")]
    [InlineData("?")]
    public void ToUrlSlug_WithOnlySymbols_ReturnsUntitled(string input)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe("/untitled");
    }

    [Fact]
    public void ToUrlSlug_WithOnlySymbolsAndSuffix_ReturnsUntitledWithRandomLetters()
    {
        var result = "!!!".ToUrlSlug(2);

        result.ShouldMatch("^/untitled-[a-z]{2}$");
    }
}
