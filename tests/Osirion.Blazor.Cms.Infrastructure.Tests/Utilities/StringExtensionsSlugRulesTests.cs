using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

public class StringExtensionsSlugRulesTests
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Hello-", "hello")]
    [InlineData("Hello - World", "hello-world")]
    [InlineData("ab cd", "ab-cd")]
    [InlineData("a b", "ab")]
    [InlineData("ab12", "ab12z")]
    [InlineData("ab-12", "ab-12z")]
    [InlineData("a1", "aa-1z")]
    [InlineData("a", "aa")]
    [InlineData("x!", "xa")]
    [InlineData("1ab", "1ab")]
    [InlineData("Ünïcode", "ncode")]
    [InlineData("snake_case_name", "snake-case-name")]
    public void ToUrlSlug_WithoutSuffix_AppliesSlugRules(string input, string expected)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1")]
    [InlineData("1 2")]
    public void ToUrlSlug_WithoutLetters_ReturnsUntitled(string input)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe("/untitled");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void ToUrlSlug_WithoutLettersAndWithSuffix_ReturnsUntitledWithRandomLetters(string input)
    {
        var result = input.ToUrlSlug(3);

        result.ShouldMatch("^/untitled-[a-z]{3}$");
    }

    [Fact]
    public void ToUrlSlug_WithSuffixAfterDigit_SeparatesSuffixWithHyphen()
    {
        var result = "ab1".ToUrlSlug(2);

        result.ShouldMatch("^ab1-[a-z]{2}$");
    }

    [Fact]
    public void ToUrlSlug_WithSuffixAfterLetter_AppendsSuffixDirectly()
    {
        var result = "a".ToUrlSlug(2);

        result.ShouldMatch("^aa[a-z]{2}$");
    }

    [Fact]
    public void ToUrlSlug_WithSuffixAfterTrailingSeparator_DropsTheSeparator()
    {
        var result = "Hello ".ToUrlSlug(4);

        result.ShouldMatch("^hello[a-z]{4}$");
    }
}
