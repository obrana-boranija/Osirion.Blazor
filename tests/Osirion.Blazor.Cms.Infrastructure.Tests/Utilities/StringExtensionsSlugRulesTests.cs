using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

public class StringExtensionsSlugRulesTests
{
    [Theory]
    [InlineData("Hello-", "hello")]
    [InlineData("Hello - World", "hello-world")]
    [InlineData("ab cd", "ab-cd")]
    [InlineData("ab12", "ab12z")]
    [InlineData("ab-12", "ab-12z")]
    [InlineData("1ab", "1ab")]
    [InlineData("snake_case_name", "snake-case-name")]
    public void ToUrlSlug_WithoutSuffix_AppliesSlugRules(string input, string expected)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe(expected);
    }

    // Known defect, pinned as current behavior: the repairs left over from the removed leading "/" corrupt short and non-ASCII input (a separator after one character is dropped, a letter is inserted at index 1, non-ASCII letters are lost). Update this expectation when it is fixed.
    [Theory]
    [InlineData("a b", "ab")]
    [InlineData("a1", "aa-1z")]
    [InlineData("a", "aa")]
    [InlineData("x!", "xa")]
    [InlineData("Ünïcode", "ncode")]
    public void ToUrlSlug_WithShortOrNonAsciiInput_ProducesACorruptedSlug(string input, string expected)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe(expected);
    }

    // Known defect, pinned as current behavior: the fallback slug still starts with "/" although real slugs no longer do. Update this expectation when it is fixed.
    [Theory]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1")]
    [InlineData("1 2")]
    public void ToUrlSlug_WithoutLetters_ReturnsUntitledWithALeadingSlash(string input)
    {
        var result = input.ToUrlSlug();

        result.ShouldBe("/untitled");
    }

    // Known defect, pinned as current behavior: the fallback slug still starts with "/" although real slugs no longer do. Update this expectation when it is fixed.
    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void ToUrlSlug_WithoutLettersAndWithSuffix_ReturnsUntitledWithALeadingSlashAndRandomLetters(string input)
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

    // Known defect, pinned as current behavior: a one-letter input gets an extra "a" inserted at index 1 before the suffix. Update this expectation when it is fixed.
    [Fact]
    public void ToUrlSlug_WithSuffixAfterASingleLetter_InsertsAnExtraLetterThenAppendsTheSuffix()
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
